using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;

namespace Wordleans.Kernel.Silo
{
    /// <summary>
    /// Sample filter to show silo wide interceptor
    /// </summary>
    public class IncomingGrainFilter : IIncomingGrainCallFilter
    {
        private readonly ILocalSiloDetails _details;

        // ctor with injected services
        public IncomingGrainFilter(ILocalSiloDetails details)
        {
            _details = details;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            // set context data
            RequestContext.Set("MACHINE-NAME", Environment.MachineName);
            RequestContext.Set("SILO-NAME", _details.Name);
            RequestContext.Set("SILO-ADDRESS", _details.GatewayAddress.Endpoint.ToString());
            RequestContext.Set("CLUSTER-ID", _details.ClusterId);

            // run grain logic
            await context.Invoke();
        }
    }
}
