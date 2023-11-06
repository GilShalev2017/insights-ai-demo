using System.Diagnostics;

namespace FRServer.Utility
{
    public class ProcessManager
    {
        private readonly Dictionary<string, Process> runningProcesses = new Dictionary<string, Process>();

        public bool TryAddProcess(string name, Process process)
        {
            if (!runningProcesses.ContainsKey(name))
            {
                runningProcesses[name] = process;
                return true;
            }
            return false;
        }

        public bool TryRemoveProcess(string name)
        {
            return runningProcesses.Remove(name);
        }

        public bool TryGetProcess(string name, out Process process)
        {
            return runningProcesses.TryGetValue(name, out process);
        }

        public List<Process> GetAllProcesses()
        {
            return runningProcesses.Values.ToList();
        }

        public void CleanAllProcesses()
        {
            runningProcesses.Clear();
        }
    }

}
