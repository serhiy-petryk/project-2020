using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.WiP {

  class DGVFormattedValue {

    // стандартный подход (перебор ячеек + вызов FormattedValue занимает 11 сек/100,000 записей (см. ниже button5)
    // перебор массива данных + вызов System.Windows.Forms.Formatter.FormatObjectInternal через Emit занимает 1,7 сек/100,000 записей (см. ниже button8)
    // подмена работы Formatter + перебор массива занимает 0,5 сек/100,000 записей (см. ниже button9)
    // простой перебор массива + вызов getter занимает 0,2 сек/100,000 записей (см. ниже button9)

    // стандартный алгоритм форматирования данных смотри в System.Windows.Forms.Formatter.FormatObjectInternal
    // Он базируется на 4-6 подходах:
    // источник строка, источник IFormatable, есть ли converter, источник IConvertible можно ли использовать Convert.ChangeType) 

    //Для фаст фильтра возможно нужно создать свой Конвертер, который будет :
    // - форматирование данных
    // - может ли форматировать данные (нужно для Check в DataReader)
    // - (DropDownTextBox- ??? стоит ли делать, может проще - это реализовать в самом контроле) поддержка стандартных значений из Базы данных

    DataGridView bodgv1;


    delegate object delFormatObject(object value, Type targetType, TypeConverter sourceConverter,
  TypeConverter targetConverter, string formatString, IFormatProvider formatProvider);

    private delFormatObject GetFormatObjectDelegate() {
      Type t = typeof(Form).Assembly.GetType("System.Windows.Forms.Formatter");
      MethodInfo mi = t.GetMethod("FormatObjectInternal", BindingFlags.Static | BindingFlags.NonPublic);

      DynamicMethod dm = new DynamicMethod("FormatObject", typeof(object),
        new Type[] { typeof(object), typeof(Type), typeof(TypeConverter), typeof(TypeConverter), typeof(string), typeof(IFormatProvider) },
        typeof(DataGridViewCell), true);
      ILGenerator il = dm.GetILGenerator();
      il.Emit(OpCodes.Ldarg, 0);
      il.Emit(OpCodes.Ldarg, 1);
      il.Emit(OpCodes.Ldarg, 2);
      il.Emit(OpCodes.Ldarg, 3);
      il.Emit(OpCodes.Ldarg, 4);
      il.Emit(OpCodes.Ldarg, 5);
      il.Emit(OpCodes.Ldnull);
      il.Emit(OpCodes.Call, mi);
      il.Emit(OpCodes.Ret);
      Type delegateType = typeof(Func<object, string>);
      delFormatObject del = (delFormatObject)dm.CreateDelegate(typeof(delFormatObject));

      DataGridViewCell cell = this.bodgv1.Rows[0].Cells[2];
      object o = cell.Value;
      Type targetType = typeof(string);
      TypeConverter sourceConverter = TypeDescriptor.GetConverter(cell.ValueType);
      TypeConverter targetConverter = TypeDescriptor.GetConverter(cell.FormattedValueType);
      string formatString = cell.GetInheritedStyle(null, 0, false).Format;
      IFormatProvider formatProvider = cell.GetInheritedStyle(null, 0, false).FormatProvider;
      object x = del.Invoke(o, targetType, sourceConverter, targetConverter, formatString, formatProvider);
      return del;
    }

    private void button5_Click(object sender, EventArgs e) {
      Stopwatch sw = new Stopwatch();
      sw.Start();
      DataGridView dgv = this.bodgv1;
      foreach (DataGridViewRow r in dgv.Rows) {
        foreach (DataGridViewCell c in r.Cells) {
          string s = c.FormattedValue.ToString();
        }
      }
      sw.Stop();
      double d1 = sw.Elapsed.TotalMilliseconds;
      int recs = dgv.Rows.Count;
    }


    private void button8_Click(object sender, EventArgs e) {
      PropertyDescriptorCollection pdc = DGVUtils.GetInternalPropertyDescriptorCollection(this.bodgv1);
      delFormatObject del = GetFormatObjectDelegate();
      Type targetType = typeof(string);
      TypeConverter targetConverter = TypeDescriptor.GetConverter(typeof(string));
      IFormatProvider formatProvider = null;
      List<TypeConverter> sourceConverters = new List<TypeConverter>();
      List<String> fmtStrings = new List<string>();
      List<PropertyDescriptor> pds = new List<PropertyDescriptor>();
      DataGridViewRow r = this.bodgv1.Rows[0];
      foreach (DataGridViewCell cell in r.Cells) {
        if (!String.IsNullOrEmpty(cell.OwningColumn.DataPropertyName)) {
          PropertyDescriptor pd = pdc[cell.OwningColumn.DataPropertyName];
          pds.Add(pd);
          fmtStrings.Add(cell.GetInheritedStyle(null, 0, false).Format);
          sourceConverters.Add(TypeDescriptor.GetConverter(cell.ValueType));
          formatProvider = cell.GetInheritedStyle(null, 0, false).FormatProvider;
        }
      }


      Stopwatch sw = new Stopwatch();
      sw.Start();
      DataGridView dgv = this.bodgv1;
      IList oo = (IList)dgv.DataSource;
      foreach (object o in oo) {
        for (int i = 0; i < pds.Count; i++) {
          object x1 = del(pds[i].GetValue(o), targetType, sourceConverters[i], targetConverter, fmtStrings[i], formatProvider);
        }
      }
      sw.Stop();
      double d1 = sw.Elapsed.TotalMilliseconds;
      int recs = dgv.Rows.Count;

    }

    private void button9_Click(object sender, EventArgs e) {
      PropertyDescriptorCollection pdc = DGVUtils.GetInternalPropertyDescriptorCollection(this.bodgv1);
      DataGridViewRow r = this.bodgv1.Rows[0];
      IFormatProvider formatProvider = null;
      List<TypeConverter> sourceConverters = new List<TypeConverter>();
      List<String> fmtStrings = new List<string>();
      List<PropertyDescriptor> pds = new List<PropertyDescriptor>();
      List<int> methods = new List<int>();
      int cnt = 0;
      foreach (DataGridViewCell cell in r.Cells) {
        if (!String.IsNullOrEmpty(cell.OwningColumn.DataPropertyName)) {
          PropertyDescriptor pd = pdc[cell.OwningColumn.DataPropertyName];
          pds.Add(pd);
          fmtStrings.Add(cell.GetInheritedStyle(null, 0, false).Format);
          sourceConverters.Add(TypeDescriptor.GetConverter(cell.ValueType));
          if (cell.ValueType == typeof(string)) methods.Add(0);
          else if (cell.ValueType is IFormattable && !String.IsNullOrEmpty(fmtStrings[cnt])) methods.Add(1);
          else if (sourceConverters[cnt] != null && sourceConverters[cnt].CanConvertFrom(typeof(string))) methods.Add(2);
          else if (cell.ValueType is IConvertible) methods.Add(3);
          else methods.Add(4);
          formatProvider = cell.GetInheritedStyle(null, 0, false).FormatProvider;
          cnt++;
        }
      }


      Stopwatch sw = new Stopwatch();
      sw.Start();
      DataGridView dgv = this.bodgv1;
      IList oo = (IList)dgv.DataSource;
      foreach (object o in oo) {
        for (int i = 0; i < pds.Count; i++) {
          object x1 = pds[i].GetValue(o);
          /*          string s;
                    if (x1 == null) s = null;
                    else {
                      switch (methods[i]) {
                        case 0: s = (string)x1; break;
                        case 1: s = ((IFormattable)x1).ToString(fmtStrings[i], formatProvider); break;
                        case 2: s = (string)sourceConverters[i].ConvertTo(x1, typeof(string)); break;
                        case 3: s = (string)Convert.ChangeType(x1, TypeCode.String, formatProvider); break;
                        default: s = x1.ToString(); break;
                      }
                    }*/
        }
      }
      sw.Stop();
      double d1 = sw.Elapsed.TotalMilliseconds;
      int recs = dgv.Rows.Count;

    }
  }
}
