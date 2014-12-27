using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
	class MainClass
	{
		public static List<HeaderNode> columns;
		public static List<int> solution = new List<int> ();
		public static HeaderNode h;
		public static string outputFileName;

		public static void Main (string[] args)
		{
			try
			{
				Initialize ();
				Console.Write ("Enter Sudoku Input File Location:");
				string inputFileName = Console.ReadLine ();
				ProcessGivens (inputFileName);
				h.Right = columns [1];
				h.Left = columns [columns.Count - 1];
				h.Down = h;
				h.Up = h;
				DancingLinks ();
				Console.Write ("Solution saved as " + outputFileName);
			}
			catch(Exception ex) {
				Console.Write (ex.Message);
			}

		}
			
		public static void ProcessGivens(string filename)
		{
			outputFileName = Path.GetFileNameWithoutExtension(filename) + ".sln.txt";
		
			string line = "";
			using (StreamReader sr = new StreamReader (filename)) {
				int i = 0;
				while ((line = sr.ReadLine ()) != null) {
					int j = 0;

					char[] chLine = line.ToCharArray ();
					foreach (char c in chLine) {
						if (c != 'X') {
							int matrixRow = (i * 9 * 9) + (j * 9) + Convert.ToInt16 (c.ToString ());

							Node coverRow = columns [(i * 9) + j + 1];
							while (coverRow.Row != matrixRow)
								coverRow = coverRow.Down;
							solution.Add (coverRow.Row);
							CoverColumn (coverRow.Column);
							Node coverColumn = coverRow;
							while ((coverColumn = coverColumn.Right) != coverRow) {
								CoverColumn (coverColumn.Column);
							}
						}
						j++;
					}
					i++;
				}
			}
		}

		public static void PrintSolution(List<int> solution)
		{
			int[] array = solution.ToArray ();
			Array.Sort (array);
			StringBuilder sb = new StringBuilder ();

			using (StreamWriter sw = new StreamWriter (new FileStream (outputFileName, FileMode.Create))) {

				int i = 0;
				foreach (int row in array) {
					int cell = row % 9;
					if (cell == 0)
						cell = 9;
					sb.Append (cell.ToString ());
					i++;
					if (i == 9) {
						sw.WriteLine (sb.ToString ());
						Console.WriteLine (sb.ToString ());
						sb.Remove (0, sb.Length);
						i = 0;
					}
				}
			}
		}

		public static void DancingLinks()
		{
			if (h == h.Right) {
				PrintSolution (solution);
				return;
			} else {
				HeaderNode c = ChooseColumn ();
				CoverColumn (c);
				Node r = c.Down;
				while (r != c) {
					solution.Add (r.Row);
					Node j = r.Right;
					while (j != r) {
						CoverColumn (j.Column);
						j = j.Right;
					}
					DancingLinks ();
					if (h == h.Right)
						return;
					solution.Remove (r.Row);
					c = r.Column;
					j = r.Left;
					while (j != r) {
						UncoverColumn (j);
						j = j.Left;
					}
					r = r.Down;
				}

				UncoverColumn ((Node)c);
				return;
			}
		}

		public static HeaderNode ChooseColumn ()
		{
			HeaderNode j = (HeaderNode)h.Right;
			HeaderNode c = j;
			int s = j.Count;
			while (j != h) {
				if (j.Count < s) {
					s = j.Count;
					c = j;
				}
				j = (HeaderNode)j.Right;
			}
			return c;
		}

		public static void CoverColumn(HeaderNode column)
		{
			column.Right.Left = column.Left;
			column.Left.Right = column.Right;

			Node coverNode = column.Down;
			while (coverNode != column) {
				Node rightNode = coverNode.Right;
				while (rightNode != coverNode) {
					rightNode.Down.Up = rightNode.Up;
					rightNode.Up.Down = rightNode.Down;
					rightNode.Column.Count--;
					rightNode = rightNode.Right;
				}
				coverNode = coverNode.Down;
			}
		}

		public static void UncoverColumn(Node column)
		{
			Node coverNode = column.Up;

			while (coverNode != (Node)column) {
				Node leftNode = coverNode.Left;
				while (leftNode != coverNode) {
					leftNode.Up.Down = leftNode;
					leftNode.Down.Up = leftNode;
					leftNode.Column.Count++;
					leftNode = leftNode.Left;
				}
				coverNode = coverNode.Up;
			}

			column.Left.Right = column;
			column.Right.Left = column;
		}

		public static void Initialize()
		{
			columns = new List<HeaderNode> ();
			string line = "";
			int i = 0;
			h = new HeaderNode ();
			h.Column = h;
			h.Count = 0;

			columns.Add (h);

			HeaderNode m = new HeaderNode ();

			using (StreamReader sr = new StreamReader ("..//..//SudokuMatrix.txt")) {
				while ((line = sr.ReadLine ()) != null) {

					//for every line. . .
					int j = 1;
					Node lastNode = null;
					char[] chLine = line.ToCharArray ();
					foreach (char c in chLine) {

						//for every value in the line

						//first row
						if (i == 0) {
							if (h.Right == null) {
								HeaderNode header = new HeaderNode (h, h, j);
								h.Left = h;
								h.Right = h;
								m = header;
								columns.Add (header);
								j++;
							} else {
								HeaderNode header = new HeaderNode (m, h, j);
								m.Right = header;
								m = header;
								columns.Add (header);
								j++;
							}

						} else {

							if (c == '1') {
								Node node = new Node ();
								node.Row = i;
								//node.Value = true;
								node.Column = columns [j];
								columns [j].Count = columns [j].Count + 1;
								Node up = new Node ();
								if (node.Column.Down != null) {
									up = node.Column.Down;
									while (up.Down.Row != 0)
										up = up.Down;
									node.Up = up;
								} else {
									node.Up = node.Column;
								}
								node.Down = node.Column;
								node.Up.Down = node;
								node.Down.Up = node;
								if (lastNode != null) {
									node.Left = lastNode;
									node.Left.Right = node;
									while (lastNode.Left.Column.Index < lastNode.Column.Index)
										lastNode = lastNode.Left;
									node.Right = lastNode;
									node.Right.Left = node;
								} else {
									node.Left = node;
									node.Right = node;
								}
								lastNode = node;

								j++;
							} else {
								j++;
							}
						}
					}
					i++;
				}
			}
		}
	}

	public class Node
	{
		public int Row{ get; set; }
		public Node Left{ get; set; }
		public Node Right{ get; set; }
		public Node Up{ get; set; }
		public Node Down{ get; set; }
		public HeaderNode Column{ get; set; }
	}
		
	public class HeaderNode : Node
	{
		public int Index{ get; set; }
		public int Count{ get; set; }

		public HeaderNode(Node left, Node right, int index)
		{
			this.Row = 0;
			this.Left = left;
			this.Right = right;
			this.Up = this;
			this.Down = this;
			this.Column = this;
			this.Index = index;
			this.Count = 0;
		}

		public HeaderNode (){
		}
	}
}
