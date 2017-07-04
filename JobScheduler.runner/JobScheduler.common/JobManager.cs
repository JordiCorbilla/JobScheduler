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

using System;
using System.Collections.Generic;

namespace JobScheduler.common
{
    public class JobManager
    {
        public LogDelegate Log { get; set; }

        private Dictionary<int, Job> Jobs { get; }

        public JobManager(LogDelegate logFunc)
        {
            Jobs = new Dictionary<int, Job>();
            Log = logFunc;
        }

        public void UnMark()
        {
            foreach (KeyValuePair<int, Job> job in Jobs)
            {
                job.Value.Marked = false;
            }
        }

        public void Load(Job job)
        {
            Job oldJob;
            if (Jobs.TryGetValue(job.Id, out oldJob))
            {
                oldJob.Update(job);
            }
            else
            {
                Jobs.Add(job.Id, job);
                Log($"New Job added with Id [{job.Id}] scheduled at: [{job.Time:hh:mm:ss tt} UTC]");
            }
        }

        public void DeleteUnMarked()
        {
            List<int> toBeRemoved = new List<int>();
            foreach (KeyValuePair<int, Job> job in Jobs)
            {
                if (!job.Value.Marked)
                {
                    toBeRemoved.Add(job.Key);
                }
            }

            foreach (int key in toBeRemoved)
            {
                Log($"Job with Id [{key}] has been removed from the Scheduler");
                Jobs.Remove(key);
            }
        }

        public void Run()
        {
            DateTime t = DateTime.UtcNow;

            foreach (KeyValuePair<int, Job> job in Jobs)
            {
                if (t > job.Value.Time && job.Value.Active)
                {
                    Log($"Running Job with Id [{job.Key}]");
                    job.Value.Run();
                }
            }
        }
    }
}
