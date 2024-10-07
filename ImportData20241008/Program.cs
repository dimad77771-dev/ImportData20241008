using System.Globalization;

namespace ImportData20241008
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string filename;
			string rezfile;

			if (args.Any())
			{
				filename = args[0];
				rezfile = args[1];
			}
			else
			{
				filename = @"D:\Projects\ImportData20241008\stmt_y4.qif";
				rezfile = @"D:\Projects\ImportData20241008\out.svc";
			}


			var text = (File.ReadAllText(filename) + "\n").Replace("\r","");
			var arrs = text.Split('^');

			var items = new List<Item>();
			foreach(var arr in arrs)
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

			var result = string.Join("\n", items.Select(x => GetLine(x)));
			File.WriteAllText(rezfile, result);
		}

		static string GetLine(Item item)
		{
			var values = new[] {item.Date, item.Payee, item.Memo, item.Tranaction, item.Category};
			var data = string.Join(",", values.Select(x => normalize(x)));
			return data;
		}

		static public string normalize(string text)
		{
			if (text == null) text = "";

			text = text.Replace("\"", "\"\"").Replace(",","\",");
			
			return "\"" + text + "\"";
		}

		class Item
		{
			public string Date;
			public string Payee;
			public string Memo;
			public string Tranaction;
			public string Category;
		}
	}
}
