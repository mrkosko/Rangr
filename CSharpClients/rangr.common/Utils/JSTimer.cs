using System;

namespace rangr.common
{
    public static class JSTimer
    {
        //http://www.dailycoding.com/Posts/easytimer__javascript_style_settimeout_and_setinterval_in_c.aspx
        public static IDisposable SetInterval(Action method, int delayInMilliseconds, bool eagerly = false)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) =>
            {
                method();
            };

            timer.Enabled = true;
            timer.Start();

            //execute callback eagerly
            if (eagerly)
            {
                method();
            }

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }


        public static void SetTimeout(Action method, int delayInMilliseconds)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) =>
            {
                method();


                timer.Stop();
                timer.Dispose();
            };

            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Start();

            //D.O: Modified this method to be self disposable

            //return timer as IDisposable;// returns a stop handle which can be used for stopping the timer, if required
        }
    }
}

