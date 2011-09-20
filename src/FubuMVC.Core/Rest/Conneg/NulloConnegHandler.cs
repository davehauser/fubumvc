using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    [MarkedForTermination]
    public class NulloConnegHandler : IConnegInputHandler, IConnegOutputHandler
    {
        public void ReadInput(CurrentRequest currentRequest, IFubuRequest request)
        {
            // do nothing
        }

        public void WriteOutput(CurrentRequest currentRequest, IFubuRequest request)
        {
            // do nothing
        }
    }
}