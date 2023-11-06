using Neurotec.Biometrics;
using Neurotec.ComponentModel;
using Neurotec.Devices;
using Neurotec.Images;
using Neurotec.Licensing;
using Neurotec.Plugins;
using Neurotec.Surveillance;
using Neurotec.Tutorials;
using Neurotec;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Neurotec.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FRProcessor
{
    class SubjectDetailsAndRange
    {
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public TimeSpan OffsetFrom { get; set; }
        public TimeSpan OffsetTo { get; set; }
        public int Score { get; set; }
    }

    class SubjectDetailsWithNameAndScores
    {
        public string Name { get; set; }
        public List<TimeRangeAndScore> TimeRangeAndScores { get; set; }
    }

    class TimeRangeAndScore
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Score { get; set; }
        public string VideoName { get; set; }
    }

    class Program
    {
        const string TimeStampFormat = "yyyy-MM-dd HH:mm:ss";

        static readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        static NSurveillance _surveillance;

        static List<string> uniqueMatched = new List<string>();
        static Dictionary<int, SubjectDetailsAndRange> subjectsMap = new Dictionary<int, SubjectDetailsAndRange>();
        static int Usage()
        {
            //Console.WriteLine("usage:");
            //Console.WriteLine("\t{0} [time in minutes] [one or more watch-list images]", TutorialUtils.GetAssemblyName());
            //Console.WriteLine("\t{0} [time in minutes] [-u url](optional) [one or more watch-list images]", TutorialUtils.GetAssemblyName());
            //Console.WriteLine("\t{0} [time in minutes] [-f folderName](optional) [one or more watch-list images]", TutorialUtils.GetAssemblyName());
            //Console.WriteLine();

            return 1;
        }

        static TimeSpan? ConvertToTimeSpan(string input)
        {
            // Split the input into parts based on the dot(.)
            string[] parts = input.Split('.');

            // Check if there are at least two parts
            if (parts.Length >= 2)
            {
                // Parse the time part (parts[1]) to a TimeSpan
                TimeSpan timeSpan = TimeSpan.Parse(parts[1]);
                return timeSpan;
            }
          
            return null;
        }
        static int Main(string[] args)
        {
            TutorialUtils.PrintTutorialHeader(args);

            if (args.Length < 2)
            {
                return Usage();
            }

            //=========================================================================
            // TRIAL MODE
            //=========================================================================
            // Below code line determines whether TRIAL is enabled or not. To use purchased licenses, don't use below code line.
            // GetTrialModeFlag() method takes value from "Bin/Licenses/TrialFlag.txt" file. So to easily change mode for all our examples, modify that file.
            // Also you can just set TRUE to "TrialMode" property in code.

            NLicenseManager.TrialMode = TutorialUtils.GetTrialModeFlag();

            //Console.WriteLine("Trial mode: " + NLicenseManager.TrialMode);

            //=========================================================================

            const string Components = "SentiVeillance";

            try
            {
                if (!NLicense.Obtain("/local", 5000, Components))
                {
                    throw new NotActivatedException(string.Format("Could not obtain licenses for components: {0}", Components));
                }

                int timeInMinutes = int.Parse(args[0]);

                //Console.WriteLine("initializing surveillance.. ");
                _surveillance = new NSurveillance();
                //Console.WriteLine("done");

                bool isUrl = args[1].Equals("-u");
                bool isFolderName = args[1].Equals("-f");

                if (_surveillance.DeviceManager.Devices.Count == 0 && !isUrl && !isFolderName)
                {
                    //Console.WriteLine("no cameras found, please plug one and try again");
                    return 0;
                }

                var biometricEngine = new NBiometricEngine();
                // set template size to large for enrolling (optional)
                biometricEngine.FacesTemplateSize = NTemplateSize.Large;
                for (int i = 1 + (isUrl || isFolderName ? 2 : 0); i < args.Length; ++i)
                {
                    string directoryPath = args[3];
                    string[] files = Directory.GetFiles(directoryPath, "*.*");

                    foreach (var file in files)
                    {
                        using (var subject = new NSubject())
                        using (var face = new NFace())
                        {
                            //Console.Write("loading image \"{0}\".. ", file);
                            face.Image = NImage.FromFile(file);
                            subject.Faces.Add(face);
                            //Console.WriteLine("done");

                            // Console.Write("creating template.. ");
                            NBiometricStatus status = biometricEngine.CreateTemplate(subject);
                            if (status == NBiometricStatus.Ok)
                            {
                                //Console.WriteLine("ok");
                                using (var template = subject.GetTemplateBuffer())
                                {
                                    //Console.Write("adding template to the surveillance watch list.. ");
                                    _surveillance.AddTemplate(file, template);
                                    //Console.WriteLine("done");
                                }
                            }
                            else
                            {
                                //Console.WriteLine("failed (status = {0}", status);
                            }
                        }
                    }
                }
                var videoName = new FileInfo(args[2]).Name;

                NCamera camera;
                if (isUrl || isFolderName)
                {
                    camera = (NCamera)ConnectDevice(_surveillance.DeviceManager, args[2], isUrl);
                }
                else
                {
                    if (_surveillance.DeviceManager.Devices.Count == 0)
                    {
                        //Console.WriteLine("no cameras found");
                        return -1;
                    }
                    camera = (NCamera)_surveillance.DeviceManager.Devices[0];
                }

                //Console.Write("adding camera to surveillance.. ");
                _surveillance.Sources.Add(new NSurveillanceSource(NSurveillanceModalityType.Faces, camera));
                //Console.WriteLine("done");

                //Console.Write("setting callbacks.. ");
                _surveillance.ImageCaptured += _surveillance_ImageCaptured;
                _surveillance.SubjectAppeared += SurveillanceSubjectAppeared;
                _surveillance.SubjectTrack += SurveillanceSubjectTrack;
                _surveillance.SubjectDisappeared += SurveillanceSubjectDisappeared;
                _surveillance.Any += SurveillanceAny;
                _surveillance.IsRunningChanged += SurveillanceIsRunningChanged;
                //Console.WriteLine("done");

                //Console.WriteLine("starting surveillance for {0} minutes.. ", timeInMinutes);

                Stopwatch stopwatch = new Stopwatch();
                var elapsedTime = "";
                stopwatch.Start();

                //_surveillance.Start();
                var res = _surveillance.StartAsync();

                autoResetEvent.WaitOne(timeInMinutes * 60000, false);

                stopwatch.Stop();
                elapsedTime = $"Processing took: {stopwatch.ElapsedMilliseconds / 1000} seconds ({stopwatch.ElapsedMilliseconds} ms)";
                //Console.WriteLine(elapsedTime);

                if (_surveillance.IsRunning)
                {
                    //Console.WriteLine("stopping surveillance.. ");
                    _surveillance.Stop();
                    if (!autoResetEvent.WaitOne(15000, false))
                        throw new Exception("Surveillance failed to stop during 15 seconds");
                }

                //Console.WriteLine($"found {uniqueMatched.Count} different people");

                //Console.ReadLine();

                Dictionary<string, SubjectDetailsWithNameAndScores> newDictionary =
             subjectsMap.Where(pair => pair.Value.Name != null)
                       .GroupBy(pair => pair.Value.Name)
                       .ToDictionary(
                           group => group.Key,
                           group => new SubjectDetailsWithNameAndScores
                           {
                               Name = group.Key,
                               TimeRangeAndScores = group.Select(pair =>
                                   new TimeRangeAndScore
                                   {
                                       From = pair.Value.From,
                                       To = pair.Value.To,
                                       Score = pair.Value.Score,
                                       VideoName = videoName
                                   }).ToList()
                           });
                string json = JsonSerializer.Serialize(newDictionary);
                Console.WriteLine($"SUMMARY:" + json);

                Console.ReadLine();
                return 0;
            }
            catch (Exception ex)
            {
                return TutorialUtils.PrintException(ex);
            }
            finally
            {
                if (_surveillance != null)
                {
                    _surveillance.Dispose();
                    _surveillance = null;
                }
            }
        }

        static bool FisrtIMg = true;
        private static void _surveillance_ImageCaptured(object sender, NSurveillanceEventArgs e)
        {
            foreach (var details in e.EventDetailsArray)
            {
                if (FisrtIMg) {
                    FisrtIMg = false;
                    Console.WriteLine($"ImageCaptured ### VideoTimeStamp {details.VideoTimeStamp} TimeStamp {details.TimeStamp}");
                }
                //var videoTs2 = details.TrackingDetailsHistory;
            }
        }
        static void SurveillanceSubjectAppeared(object sender, NSurveillanceEventArgs e)
        {
            foreach (var details in e.EventDetailsArray)
            {
                Console.WriteLine($"SubjectAppeared ### VideoTimeStamp {details.VideoTimeStamp} TimeStamp {details.TimeStamp}");
                var subjectDetailsAndRange = new SubjectDetailsAndRange
                {
                    From = details.TimeStamp.ToString(TimeStampFormat),
                    OffsetFrom = details.VideoTimeStamp
                };
                subjectsMap.Add(details.TraceIndex, subjectDetailsAndRange);
                //Console.WriteLine("[{0}] [{1}] Subject appeared", details.TimeStamp.ToString(TimeStampFormat), details.TraceIndex);
                details.Dispose();
            }
        }

        static void SurveillanceSubjectTrack(object sender, NSurveillanceEventArgs e)
        {
            foreach (var details in e.EventDetailsArray)
            {
                //var videoTs = details.VideoTimeStamp;
                //Console.WriteLine($"SubjectTrack: {videoTs}");
                if (details.AttributesContainsDetails)
                {
                    //Console.Write("[{0}] [{1}] Subject tracked", details.TimeStamp.ToString(TimeStampFormat), details.TraceIndex);
                    NSurveillanceEventDetails.BestMatchCollection matches = details.BestMatches;
                    if (matches.Count == 0)
                    {
                        // Console.WriteLine();
                    }
                    else
                    {
                        var id = details.TraceIndex;
                        var faceName = new FileInfo(matches[0].Id).Name;
                        subjectsMap[id].Name = faceName;
                        subjectsMap[id].Score = matches[0].Score;
                        if (!uniqueMatched.Contains(matches[0].Id))
                        {
                            uniqueMatched.Add(matches[0].Id);
                            Console.WriteLine($"SubjectTrack ### VideoTimeStamp {details.VideoTimeStamp} TimeStamp {details.TimeStamp}");
                            Console.WriteLine($"FACEDETECTED:{faceName}-SCORE:{matches[0].Score}");
                        }
                        //Console.WriteLine(", best match with template id = {0} collected score = {1}", matches[0].Id, matches[0].Score);
                    }
                }
                details.Dispose();
            }
        }

        static void SurveillanceSubjectDisappeared(object sender, NSurveillanceEventArgs e)
        {
            foreach (var details in e.EventDetailsArray)
            {
                Console.WriteLine($"SubjectDisappeared: VideoTimeStamp: {details.VideoTimeStamp} TimeStamp {details.TimeStamp}");
                subjectsMap[details.TraceIndex].To = details.TimeStamp.ToString(TimeStampFormat);
                subjectsMap[details.TraceIndex].OffsetTo = details.VideoTimeStamp;
                //Console.WriteLine("[{0}] [{1}] Subject disappeared", details.TimeStamp.ToString(TimeStampFormat), details.TraceIndex);
                NSurveillanceEventDetails.BestMatchCollection matches = details.BestMatches;
                if (matches.Count == 0)
                {
                    //Console.WriteLine(", subject not found in the watch list");
                }
                else
                {
                    //Console.WriteLine(", subject found in the watch list, best matches (score, template id):");
                    for (int i = 0; i < matches.Count; ++i)
                    {
                        //Console.WriteLine("\t{0}. {1} {2}", i, matches[i].Score, matches[i].Id);
                    }
                }
                details.Dispose();
            }
        }

        static NDevice ConnectDevice(NDeviceManager deviceManager, string url, bool isUrl)
        {
            NPlugin plugin = NDeviceManager.PluginManager.Plugins["Media"];
            if (plugin.State == NPluginState.Plugged && NDeviceManager.IsConnectToDeviceSupported(plugin))
            {
                NParameterDescriptor[] parameters = NDeviceManager.GetConnectToDeviceParameters(plugin);
                var bag = new NParameterBag(parameters);
                if (isUrl)
                {
                    bag.SetProperty("DisplayName", "IP Camera");
                    bag.SetProperty("Url", url);
                }
                else
                {
                    bag.SetProperty("DisplayName", "Video file");
                    bag.SetProperty("FileName", url);
                }
                return deviceManager.ConnectToDevice(plugin, bag.ToPropertyBag());
            }
            throw new Exception("Failed to connect specified device!");
        }

        static void SurveillanceAny(object sender, NSurveillanceEventArgs e)
        {
            foreach (var details in e.EventDetailsArray)
            {
                if (details.Error != null)
                {
                    TutorialUtils.PrintException(details.Error);
                }
                details.Dispose();
            }
        }

        static void SurveillanceIsRunningChanged(object sender, EventArgs e)
        {
            bool running = _surveillance.IsRunning;
            if (running)
            {
                Console.WriteLine("Surveillance has started");
            }
            else
            {
                Console.WriteLine("Surveillance has stopped");
                autoResetEvent.Set();
            }
        }

        //private TimeSpan GetMediaTimeSpan(VideoSource videoSource, TimeSpan? clockValue = null)
        //{
        //    // Convert media player time to be same as surveillance returned
        //    var startTime = Media.PlaybackStartTime;
        //    var clock = clockValue ?? Media.FramePosition;
        //    if (startTime.HasValue)
        //        clock -= startTime.Value;
        //    if (videoSource.StartTime != default) // Timestamp from firs ImageCaptured callback
        //        clock += videoSource.StartTime;
        //    return clock;
        //}

    }
}
