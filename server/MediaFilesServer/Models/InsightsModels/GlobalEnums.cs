using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActInfra
{

    public class GlobalEnums
    {

        public abstract class ActEnums
        {
            private readonly string _value;
            public ActEnums(string value) => _value = value;
            public string Value => _value;
            public static bool operator ==(ActEnums j1, ActEnums j2) => j1._value == j2._value;
            public static bool operator !=(ActEnums j1, ActEnums j2) => !(j1 == j2);
            public override bool Equals(object? obj)
            {
                if (obj == null)
                    return false;

                return obj is ActEnums j && j._value == _value;
            }
            public override string ToString() => _value;
            public override int GetHashCode() => _value.GetHashCode();
        }

        public class JobStatus : ActEnums
        {
            static public readonly JobStatus New = new("New");
            static public readonly JobStatus Restart = new("Restart");
            static public readonly JobStatus Pending = new("Pending");
            static public readonly JobStatus Processing = new("Processing");
            static public readonly JobStatus Failed = new("Failed");
            static public readonly JobStatus Delivering = new("Delivering");
            static public readonly JobStatus Succeeded = new("Succeeded");
            static public readonly JobStatus Canceled = new("Canceled");
            static public readonly JobStatus Canceling = new("Canceling");

            public JobStatus(string value) : base(value) { }

        }

        public class JobDeliveryType : ActEnums
        {
            static public readonly JobDeliveryType Catalog = new("Catalog");
            static public readonly JobDeliveryType CentralServer = new("CentralServer");

            public JobDeliveryType(string value) : base(value) { }

        }

        public class MicroServiceType : ActEnums
        {
            static public readonly MicroServiceType HlsOrchMicroService = new("HlsOrchMicroService");
            static public readonly MicroServiceType ClipMicroservice = new("ClipMicroservice");
            static public readonly MicroServiceType IntelligenceMicroservice = new("IntelligenceMicroservice");

            public MicroServiceType(string value) : base(value) { }
        }

        public class RecorderStatisticsType : ActEnums
        {
            static public readonly RecorderStatisticsType Live = new("Live");
            static public readonly RecorderStatisticsType LastHour = new("LastHour");

            public RecorderStatisticsType(string value) : base(value) { }
        }


        public class HLSDownloadContentTypeEnum : ActEnums
        {
            static public readonly HLSDownloadContentTypeEnum Ext_X_Stream_Inf = new("ExtStream");
            static public readonly HLSDownloadContentTypeEnum Ext_X_Media = new("ExtMedia");

            public HLSDownloadContentTypeEnum(string value) : base(value) { }
        }
    }

}
