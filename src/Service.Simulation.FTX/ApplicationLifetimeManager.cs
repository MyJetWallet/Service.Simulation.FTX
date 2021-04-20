﻿using System;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.Simulation.FTX.Services;

namespace Service.Simulation.FTX
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly OrderBookManager _bookManager;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger,
            OrderBookManager bookManager)
            : base(appLifetime)
        {
            _logger = logger;
            _bookManager = bookManager;
        }

        protected override void OnStarted()
        {
            using var _ = MyTelemetry.StartActivity("Application started");
            
            _logger.LogInformation("OnStarted has been called.");
            _bookManager.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _bookManager.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
