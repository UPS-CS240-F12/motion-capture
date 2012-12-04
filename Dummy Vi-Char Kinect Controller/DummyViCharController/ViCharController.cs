using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Controller_Core.Client
{
    public class ViCharController : IDisposable
    {
        public bool Moving
        {
            get
            {
                return (DateTime.Now - movingActivation) < TimeSpan.FromMilliseconds(movingDuration);
            }
        }

        public bool TurningLeft
        {
            get
            {
                return (DateTime.Now - movingActivation) < TimeSpan.FromMilliseconds(turningLeftDuration);
            }
        }

        public bool TurningRight
        {
            get
            {
                return (DateTime.Now - movingActivation) < TimeSpan.FromMilliseconds(turningRightDuration);
            }
        }

        public bool Jumping
        {
            get
            {
                return (DateTime.Now - movingActivation) < TimeSpan.FromMilliseconds(jumpingDuration);
            }
        }

        private DateTime movingActivation;
        private int movingDuration;

        private DateTime turningLeftActivation;
        private int turningLeftDuration;

        private DateTime turningRightActivation;
        private int turningRightDuration;

        private DateTime jumpingActivation;
        private int jumpingDuration;

        private DateTime attackActivation;
        private int attackDuration;

        private DateTime shieldActivation;
        private int shieldDuration;

        Thread gestureHandling;

        public ViCharController() 
        {
            this.movingDuration = 500;
            this.turningLeftDuration = 125;
            this.turningRightDuration = 125;
            this.jumpingDuration = 500;
            this.attackDuration = 500;
            this.shieldDuration = 500;

            gestureHandling = new Thread(this.waitForGestures);
            gestureHandling.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);      
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (gestureHandling.IsAlive)
                {
                    gestureHandling.Abort();
                }
                gestureHandling.Join();
            }
        }

        private void waitForGestures()
        {
            try
            {
                using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", "viCharControllerPipe", PipeDirection.In, PipeOptions.Asynchronous))
                {
                    try
                    {
                        clientPipe.Connect();
                    }
                    catch (Exception e) { }
                    
                    using (StreamReader sr = new StreamReader(clientPipe))
                    {
                        string controllerEvent;
                        while ((controllerEvent = sr.ReadLine()) != null)
                        {
                            Console.WriteLine(controllerEvent);
                            switch (controllerEvent)
                            {
                                case "Moving": movingActivation = DateTime.Now;
                                    break;
                                case "TurningLeft": turningLeftActivation = DateTime.Now;
                                    break;
                                case "TurningRight": turningRightActivation = DateTime.Now;
                                    break;
                                case "Jumping": jumpingActivation = DateTime.Now;
                                    break;
                                case "VoiceAttack": attackActivation = DateTime.Now;
                                    break;
                                case "VoiceShield":
                                    break;
                                default: Console.WriteLine("Unknown:" + controllerEvent);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                return;
            }
        }
    }
}
