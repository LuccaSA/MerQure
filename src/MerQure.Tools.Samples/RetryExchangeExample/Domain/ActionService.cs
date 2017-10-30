using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryExchangeExample.Domain
{
    public class ActionService
    {
        private static int _fakeError;

        private readonly ISomethingService _somethingService;

        public ActionService(ISomethingService somethingService)
        {
            _somethingService = somethingService;
        }

        public void SendNewSomething()
        {
            _somethingService.Send(new Something()
            {
                Name = "MerQure Tools"
            });
        }

        public void Consume()
        {
            _somethingService.Consume(async (object sender, Something something) => await OnSomethingReceived(sender, something));
        }

        public async Task OnSomethingReceived(object sender, Something something)
        {
            try
            {
                await ManageNewSomething(something);
            }
            catch (SomethingException)
            {
                _somethingService.RetryLater(something);
            }
            catch (Exception e)
            {
                _somethingService.SendOnError(something);
                //...
            }
        }

        private async Task ManageNewSomething(Something something)
        {
            _fakeError++;

            int value = new Random().Next(1000, 3000);
            await Task.Delay(value);
            if (_fakeError % 15 == 0)
            {
                throw new Exception("This is an unmanageable exception, like NPE");
            }
            else if (_fakeError % 5 == 0)
            {
                throw new SomethingException("This is an \"retry later\" exception");
            }
            else
            {
                //everything ok
                _somethingService.Acknowlegde(something);
            }
        }
    }
}
