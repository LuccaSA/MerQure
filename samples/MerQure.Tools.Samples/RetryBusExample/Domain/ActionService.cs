using System;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryBusExample.Domain;

public class ActionService
{
    private int _fakeError;

    private readonly ISampleService _sampleService;

    public ActionService(ISampleService sampleService)
    {
        _sampleService = sampleService;
    }

    public Task SendNewSampleAsync()
    {
        return _sampleService.SendAsync(new Sample()
        {
            Name = "MerQure Tools"
        });
    }

    public async Task ConsumeAsync()
    {
        await _sampleService.ConsumeAsync(async (object sender, Sample sample) => await OnSampleReceivedAsync(sender, sample));
    }

    public async Task OnSampleReceivedAsync(object sender, Sample sample)
    {
        try
        {
            await ManageNewSampleAsync(sample);
        }
        catch (SampleException)
        {
            await _sampleService.RetryLaterAsync(sample);
        }
        catch (Exception)
        {
            await _sampleService.SendOnErrorAsync(sample);
            //...
        }
    }

    private async Task ManageNewSampleAsync(Sample sample)
    {
        _fakeError++;

        int value = new Random().Next(1000, 3000);
        await Task.Delay(value);
        if (_fakeError % 15 == 0)
        {
            throw new SampleException("This is an unmanageable exception, like NPE");
        }
        else if (_fakeError % 5 == 0)
        {
            throw new SampleException("This is an \"retry later\" exception");
        }
        else
        {
            //everything ok
            await _sampleService.AcknowlegdeAsync(sample);
        }
    }
}