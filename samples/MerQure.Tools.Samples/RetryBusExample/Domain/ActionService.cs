using System;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryBusExample.Domain
{
	public class ActionService
    {
        private int _fakeError;

        private readonly ISampleService _sampleService;

        public ActionService(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public void SendNewSample()
        {
            _sampleService.Send(new Sample()
            {
                Name = "MerQure Tools"
            });
        }

        public void Consume()
        {
            _sampleService.Consume(async (object sender, Sample sample) => await OnSampleReceived(sender, sample));
        }

        public async Task OnSampleReceived(object sender, Sample sample)
        {
            try
            {
                await ManageNewSample(sample);
            }
            catch (SampleException)
            {
                _sampleService.RetryLater(sample);
            }
            catch (Exception)
            {
                _sampleService.SendOnError(sample);
                //...
            }
        }

        private async Task ManageNewSample(Sample sample)
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
                _sampleService.Acknowlegde(sample);
            }
        }
    }
}
