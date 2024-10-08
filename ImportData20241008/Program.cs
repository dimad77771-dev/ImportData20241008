using CsvHelper;
using System.Globalization;
using System.Text;


namespace ImportData20241008
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var file_1 = @"D:\Projects\ImportData20241008\stmt_y4.qif";
			var file_2 = @"D:\Projects\ImportData20241008\stmt_y4.csv";
			if (args.Length >= 2)
			{
				file_1 = args[0];
				file_2 = args[1];
			}
			var ext_1 = Path.GetExtension(file_1).ToLower();
			var ext_2 = Path.GetExtension(file_2).ToLower();

			if (ext_1 == ".qif" && ext_2 == ".csv")
			{
				Proc1(file_1, file_2);
			}
			else if (ext_1 == ".csv" && ext_2 == ".qif")
			{
				Proc2(file_1, file_2);
			}
			else throw new Exception("qif->csv OR csv->qif");

			//Proc1(@"D:\Projects\ImportData20241008\stmt_y4.qif", @"D:\Projects\ImportData20241008\out.csv"); 
			//Proc2(@"D:\Projects\ImportData20241008\out.csv", @"D:\Projects\ImportData20241008\stmt_y41.qif");
		}


		static void Proc1(string giffile, string csvfile)
		{
			var text = (File.ReadAllText(giffile) + "\n").Replace("\r", "");
			var arrs = text.Split('^');

			var items = new List<Item>();
			foreach (var arr in arrs)
			{
				var g = arr;
				var lines = arr.Split("\n").Where(x => !string.IsNullOrEmpty(x)).ToList();
				if (!lines.Any()) continue;

				var item = new Item();
				items.Add(item);
				foreach (var line in lines)
				{
					if (line.StartsWith("D"))
					{
						//item.Date = DateTime.ParseExact(line.Substring(1),@"MM\/dd\/yyyy", CultureInfo.InvariantCulture);
						item.Date = line.Substring(1);
					}
					else if (line.StartsWith("P"))
					{
						item.Memo = line.Substring(1);
					}
					else if (line.StartsWith("T"))
					{
						//item.Tranaction = Decimal.Parse(line.Substring(1).Replace(",","."), CultureInfo.InvariantCulture);
						item.Tranaction = line.Substring(1);
					}
					else if (line.StartsWith("C"))
					{
						item.Category = line.Substring(1);
					}
				}
			}


			using (var writer = new StreamWriter(csvfile))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteRecords(items);
			}
		}

		static void Proc2(string csvfile, string giffile)
		{
			using (var reader = new StreamReader(csvfile))
			using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				var records = csv.GetRecords<Item>();
				var outs = new StringBuilder();

				foreach (var record in records)
				{
					outs.AppendLine("D" + record.Date);
					outs.AppendLine("P" + record.Payee);
					outs.AppendLine("M" + record.Memo);
					outs.AppendLine("T" + record.Tranaction);
					outs.AppendLine("C" + record.Category);
					outs.AppendLine("^");
				}

				File.WriteAllText(giffile, outs.ToString());
			}
		}


		public class Item
		{
			public string Date { get; set; }
			public string Payee { get; set; }
			public string Memo { get; set; }
			public string Tranaction { get; set; }
			public string Category { get; set; }
		}
	}
}
