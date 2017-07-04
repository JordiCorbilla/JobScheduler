# JobScheduler
Simple Job Scheduler for .NET.

The idea behind this simple Job Scheduler is to provide a way of running specific jobs at certain times during the day. The Scheduler runs every 10s and loads the list of jobs to run at a specific time of the day. Once the time is met, the job is executed using a task.
As the loading of the tasks happens every 10s, changes are easily detected and the Scheduler will allocate/deallocate tasks at will.

Example usage is as follows:

```C#
class Program
    {
        private static JobManager JobManager { get; set; }
        private static readonly object Mylock = new object();

        static void Main()
        {
            JobManager = new JobManager(Dump);

            Timer timer = new Timer
            {
                Interval = 10000, //Every 10 seconds
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
            Dump("Starting Job Scheduler by [Jordi Corbilla]");
            Console.Read();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Load all the jobs
            if (Monitor.TryEnter(Mylock))
            {
                try
                {
                    JobManager.Run();
                    LoadJobs();
                }
                finally
                {
                    Monitor.Exit(Mylock);
                }
            }
            Dump($"[{DateTime.UtcNow:hh:mm:ss tt} UTC] Running.");
        }

        private static void LoadJobs()
        {
            IDal sqlDal = new SqlDal();
            Collection<TimeSchedule> timeScheduleList = sqlDal.GetListTimeSchedules();

            JobManager.UnMark();
            foreach (var time in timeScheduleList)
            {
                DateTime today = DateTime.UtcNow;
                //Compose the new date
                DateTime newTime = new DateTime(today.Year, today.Month, today.Day, time.Time.Hour, time.Time.Minute, time.Time.Second);
                if (newTime > today) //then we can add it to the list
                {
                    JobTest job = new JobTest(time.Id, newTime) {Marked = true};
                    JobManager.Load(job);
                }
            }
            JobManager.DeleteUnMarked();
        }

        private static void Dump(string message)
        {
            //Add anything here
            Console.WriteLine(message);
        }
    }
```

Your new Job needs to descend from Job class and in there specify the task to be run:
```C#
    public class JobTest : Job
    {
        public JobTest(int id, DateTime time) : base(Dump)
        {
            Id = id;
            Time = time;
        }

        private static void Dump(string message)
        {
            //Add anything here
            Console.WriteLine(message);
        }

        public override void Run()
        {
            Task.Factory.StartNew(() => { Console.WriteLine($"[{DateTime.Now:hh:mm:ss tt} UTC] It runs JobTest!"); });
            Active = false;
        }
    }
```
Sample log of the Scheduler running is as follows:
```Bash
[10:46:07 PM UTC] Running.
[10:46:17 PM UTC] Running.
[10:46:27 PM UTC] Running.
New Job added with Id [3] scheduled at: [10:55:00 PM UTC]
[10:46:37 PM UTC] Running.
Job has been updated with Id [3], Old Time: [10:55:00 PM UTC], New Time: [10:50:00 PM UTC]
[10:46:47 PM UTC] Running.
[10:46:57 PM UTC] Running.
[10:47:07 PM UTC] Running.
..
..
[10:50:07 PM UTC] It runs JobTest!.
```

**Licence**
-------

    The MIT License (MIT)
    
    Copyright (c) 2017 Jordi Corbilla
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    
    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
