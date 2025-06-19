
using System;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryBusExample.Domain;

public interface ISampleService
{
    Task SendAsync(Sample sample);
    Task ConsumeAsync(EventHandler<Sample> onSampleReceived);
    Task AcknowlegdeAsync(Sample sample);
    Task RetryLaterAsync(Sample sample);
    Task SendOnErrorAsync(Sample sample);
}