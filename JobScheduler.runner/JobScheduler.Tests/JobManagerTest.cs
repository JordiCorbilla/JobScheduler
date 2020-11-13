﻿using JobScheduler.common;
using System;
using NUnit.Framework;

namespace JobScheduler.Tests
{
    [TestFixture]
    public class JobManagerTest
    {
        [Test]
        public void TestLoad()
        {
            JobManager jobManager = new JobManager(Dump);
            jobManager.UnMark();
            DateTime today = DateTime.UtcNow;
            JobTest test1 = new JobTest(1, today.AddSeconds(35));
            JobTest2 test2 = new JobTest2(2, today.AddSeconds(42));
            jobManager.Load(test1);
            jobManager.Load(test2);
            jobManager.DeleteUnMarked();
        }

        private void Dump(string message)
        {
            //Add anything here
            Console.WriteLine(message);
        }
    }
}
