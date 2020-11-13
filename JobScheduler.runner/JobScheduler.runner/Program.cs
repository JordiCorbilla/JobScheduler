﻿//  Copyright (c) 2017, Jordi Corbilla
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//
//  - Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//  - Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//  - Neither the name of this library nor the names of its contributors may be
//    used to endorse or promote products derived from this software without
//    specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
//  LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
//  SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.

using JobScheduler.common;
using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace JobScheduler.runner
{
    static class Program
    {
        private static JobManager JobManager { get; set; }
        private static readonly object MyLock = new object();
        private static DateTime Today { get; set; }
        static void Main()
        {
            Today = DateTime.UtcNow;
            JobManager = new JobManager(Dump);

            Timer timer = new Timer
            {
                Interval = 10000, //Every 10 seconds
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;

            Dump(@"       _         _        _____        _                _         _             ");
            Dump(@"      | |       | |      / ____|      | |              | |       | |            ");
            Dump(@"      | |  ___  | |__   | (___    ___ | |__    ___   __| | _   _ | |  ___  _ __ ");
            Dump(@"  _   | | / _ \ | '_ \   \___ \  / __|| '_ \  / _ \ / _` || | | || | / _ \| '__|");
            Dump(@" | |__| || (_) || |_) |  ____) || (__ | | | ||  __/| (_| || |_| || ||  __/| |   ");
            Dump(@"  \____/  \___/ |_.__/  |_____/  \___||_| |_| \___| \__,_| \__,_||_| \___||_|   ");
            Dump(@"                                                                                ");
            Dump("Starting Job Scheduler by [Jordi Corbilla]");
            Dump("");
            Console.Read();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Load all the jobs
            if (Monitor.TryEnter(MyLock))
            {
                try
                {
                    JobManager.Run();
                    LoadJobs();
                }
                finally
                {
                    Monitor.Exit(MyLock);
                }
            }
            Dump($"[{DateTime.UtcNow:hh:mm:ss tt} UTC] Running.");
        }

        //Separate method to load the list of jobs.
        //This could come from a DB for example
        private static void LoadJobs()
        {
            JobManager.UnMark();
            JobTest test1 = new JobTest(1, Today.AddSeconds(35));
            JobTest2 test2 = new JobTest2(2, Today.AddSeconds(42));
            JobManager.Load(test1);
            JobManager.Load(test2);
            JobManager.DeleteUnMarked();
        }
        
        private static void Dump(string message)
        {
            //Add anything here
            Console.WriteLine(message);
        }        
    }
}
