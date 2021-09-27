namespace DGWnd
{
  public class MultiThreading
  {
    /*
     * ToDo: Bug: 
     * 1. При закриті форми/програми, якщо виконуються фонові задачі, виникають помилки.
     *
     *
     * Тестувати краще всього для операцій FastFilter набору даних GlDocLine
     * Затримка 3-10 секунд для 1'000'000 recs
     *
     * Onethread mode is bad: вся програма блокується
     *
     * Multithread mode:
     * ResetBindings (BindingList) виконується десь 3 секунди для GlDocLine і взаємодіє з DataGridView і його потрібно визивати в UI thread
     * 1. Потрібно підготовку даних роботи в окремому потоці.
     * 2. Перед підготовкою даних зробити dgv.Enabled = false, тому що будуть помилки при руху мишки, активації клітинок для dgv.
     * 3. Коли оброблюються дані можна зробити сірим dgv (see https://stackoverflow.com/questions/8715459/disabling-or-greying-out-a-datagridview)
     * Але при простих наборах даних при цьому є мерехтіння (тест на FastFilter for MastAltCoA data set)
     * Мерехтіння визване через зміну Control.EnableHeadersVisualStyles
     * -------- Так що краще цього не робити.
     * Можна зробити окремий малюнок, який буде зявлятися при обробці даних (див. https://www.codeproject.com/Articles/1220062/Display-loading-indicator-in-Windows-Form-app-16)
     *
     * Висновок. При підготовці даних робимо:
     * 1. dgv.enabled = false;
     * 2. Робимо сірим dgv (властивість Control.EnableHeadersVisualStyles не чіпаємо)
     * 3. В рядку статуса форми виводимо малюнок.
     */

    /*
     * Приклад реалізації (з Cancel, Timer) дивись WpfData.TestingDataX64 project (file LoaderTest64.cs):
     *
     *[TestMethod]
    public void ParallelWithCancel()
    {
      var r1 = Common.MemoryUsedInKB;

      var cts = new CancellationTokenSource();
   //   var timer = new System.Timers.Timer() { Interval = 30000 };
    //  timer.Elapsed += (sender, args) =>
    //  {
    //    cts.Cancel();
  //      // var lk = 0;
//      };
     // timer.Start();

    var sw = new Stopwatch();
    sw.Start();

    var tasks = new List<Task>
    {
      Task.Factory.StartNew(MastCoAAlt.Init),
      Task.Factory.StartNew(MastCoA.Init),
      Task.Factory.StartNew(GlDocList.Init),
      Task.Factory.StartNew(GlDocLine.Init)
    };

    // cts.CancelAfter(1000);

    var result = "";

    Task.Factory.StartNew(() =>
    {
      System.Threading.Thread.Sleep(100);
      cts.Cancel();
    });

    try
  {
  Task.WaitAll(tasks.ToArray(), cts.Token);
  // Task.WaitAll(tasks.ToArray());
  }
  catch (OperationCanceledException)
  {
  result += "\r\nDownload canceled.\r\n";
  }
  catch (Exception ex)
  {
  result += "\r\n"+ ex.ToString()+".\r\n";
  }
  // await Task.WhenAll(tasks.ToArray());

  sw.Stop();
  var a1 = sw.Elapsed.TotalMilliseconds;
  var r2 = Common.MemoryUsedInKB;
  var r21 = r2 - r1;
}

     *
     */

  }
}
