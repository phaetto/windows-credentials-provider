using System;
using System.Threading;
using CredentialProvider.Interop;

namespace WindowsCredentialProviderTest.OnDemandLogon
{
    public delegate void TimerEndedDelegate();

    public sealed class TimerOnDemandLogon : IDisposable
    {
        private readonly ICredentialProviderEvents credentialProviderEvents;
        private readonly ICredentialProviderCredentialEvents credentialProviderCredentialEvents;
        private readonly ICredentialProviderCredential credentialProviderCredential;
        private readonly uint textFieldId;
        private readonly uint adviceContext;
        private readonly Timer timer;
        private int secondsLeft = 6;

        public event TimerEndedDelegate TimerEnded;

        public TimerOnDemandLogon(
            ICredentialProviderEvents credentialProviderEvents,
            ICredentialProviderCredentialEvents credentialProviderCredentialEvents,
            ICredentialProviderCredential credentialProviderCredential,
            uint textFieldId,
            uint adviceContext  // void*
        )
        {
            this.credentialProviderEvents = credentialProviderEvents;
            this.credentialProviderCredentialEvents = credentialProviderCredentialEvents;
            this.credentialProviderCredential = credentialProviderCredential;
            this.textFieldId = textFieldId;
            this.adviceContext = adviceContext;
            timer = new Timer(TimerCallback, this, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        private static void TimerCallback(object state)
        {
            var timerOnDemandLogon = (TimerOnDemandLogon) state;
            --timerOnDemandLogon.secondsLeft;

            if (timerOnDemandLogon.secondsLeft > 0)
            {
                timerOnDemandLogon.credentialProviderCredentialEvents.SetFieldString(
                    timerOnDemandLogon.credentialProviderCredential,
                    timerOnDemandLogon.textFieldId,
                    $"Seconds passed {timerOnDemandLogon.secondsLeft} (in awesomeness)");
            }
            else
            {
                timerOnDemandLogon.timer.Change(int.MaxValue, int.MaxValue);
                timerOnDemandLogon.credentialProviderEvents.CredentialsChanged(timerOnDemandLogon.adviceContext);
                timerOnDemandLogon.OnTimerEnded();
            }
        }

        public void Dispose()
        {
            timer?.Change(int.MaxValue, int.MaxValue);
            timer?.Dispose();
        }

        private void OnTimerEnded()
        {
            TimerEnded?.Invoke();
        }
    }
}