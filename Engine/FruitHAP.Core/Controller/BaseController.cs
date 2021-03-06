﻿using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FruitHAP.Common.EventBus;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Core.Controller
{
    public abstract class BaseController : IController
    {
        protected readonly ILogger logger;
        protected readonly IEventBus eventBus;

        protected abstract void StartController();
        protected abstract void StopController();
        protected abstract void DisposeController();

        public string Name
        {
            get { return AssemblyHelpers.GetAssemblyTitle(this.GetType().Assembly); }
        }

        public string Description
        {
            get { return AssemblyHelpers.GetAssemblyDescription(this.GetType().Assembly); }
        }

        public string Version
        {
            get { return AssemblyHelpers.GetAssemblyVersion(this.GetType().Assembly); }
        }

        public bool IsStarted { get; private set; }

        private bool disposedValue = false; // To detect redundant calls

		protected BaseController(ILogger logger, IEventBus eventBus)
        {
            this.logger = logger;
			this.eventBus = eventBus;
        }

        public void Start()
        {            
            try
            {
                if (!IsStarted)
                {
                    logger.InfoFormat("Starting controller {0}", this);
                    StartController();
                    IsStarted = true;
                    logger.InfoFormat("Started controller {0}", this);
                }
            }
            catch (Exception ex)
            {
                IsStarted = false;
                logger.ErrorFormat(ex,"Could not start controller {0}:", this);
            }
        }

        public void Stop()
        {
            logger.InfoFormat("Stopping controller {0}", this);
            StopController();
            logger.InfoFormat("Stopped controller {0}", this);
            IsStarted = false;
        }


        #region IDisposable Support
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    logger.DebugFormat("Disposing controller {0}", this.GetType());
                    DisposeController();
                }                

                disposedValue = true;
            }
        }


        public void Dispose()
        {            
            Dispose(true);            
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Version);
        }
    }
}
