//  Copyright (c) 2017, Jordi Corbilla
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
    class Program
    {
        private static JobManager jobManager { get; set; }
        private static readonly object Mylock = new object();
        private static DateTime today { get; set; }
        static void Main(string[] args)
        {
            today = DateTime.Now;
            jobManager = new JobManager();

            Timer timer = new Timer
            {
                Interval = 10000, //Every 10 seconds
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
            Console.WriteLine("Starting Job Scheduler by Jordi Corbilla");
            Console.Read();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Load all the jobs
            if (Monitor.TryEnter(Mylock))
            {
                try
                {
                    LoadJobs();
                    jobManager.Run();
                }
                finally
                {
                    Monitor.Exit(Mylock);
                }
            }
            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss")} Running!");
        }

        //Separate method to load the list of jobs.
        //This could come from a DB for example
        private static void LoadJobs()
        {
            JobTest test1 = new JobTest(1, today.AddMinutes(2));
            JobTest2 test2 = new JobTest2(2, today.AddMinutes(3));
            jobManager.Load(test1);
            jobManager.Load(test2);
        }
    }
}
