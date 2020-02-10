//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lights_Out
{
	public partial class MainForm : Form
	{
		int count, HowPictureBox;
		Dictionary <int,Point> ExistingBox;
		Dictionary <int,int> HachCode_OfBox;
		int _Widht = 600, _Height = 600, _SizeOfSquare = 60;
		public MainForm()
		{
			InitializeComponent();
			
			
		}

		void _genmap(int N)
		{
			Controls.Clear();
			
			_Widht = _SizeOfSquare * N;
			_Height = _SizeOfSquare * N;
			
			Height = (int)(_Height * 1.3);
			Width = (int)(_Widht * 1.2);

			
			
			
			for (int i = 0; i <= _Height / _SizeOfSquare; i++) {
				PictureBox field = new PictureBox();
				field.Size = new Size(_Widht, 1);
				field.Location = new Point(0, _SizeOfSquare * i);
				field.BackColor = Color.Gray;
				Controls.Add(field);
			}
			
			for (int i = 0; i <= _Widht / _SizeOfSquare; i++) {
				PictureBox field = new PictureBox();
				field.Size = new Size(1, _Height);
				field.Location = new Point(_SizeOfSquare * i, 0);
				field.BackColor = Color.Gray;
				Controls.Add(field);
			}
			
			
			
			count = Controls.Count;
			HowPictureBox = N * N;
			
			HachCode_OfBox = new Dictionary<int,int>();
			ExistingBox = new Dictionary<int, Point>();
			var ExistingBox2 = new Dictionary<int, Point>();
			for (int p = 1, i = 0; i < N; i++) {
				for (int j = 0; j < N; p++, j++) {
					
					PictureBox field = new PictureBox();
					
					field.Size = new Size(_SizeOfSquare, _SizeOfSquare);
					field.Location = new Point(i * _SizeOfSquare, j * _SizeOfSquare);
					field.BackColor = Color.Lime;
					field.Click += (MyClick);
					Controls.Add(field);
					
					ExistingBox.Add(Controls.Count - 1, new Point(i * _SizeOfSquare, j * _SizeOfSquare));
					HachCode_OfBox.Add(field.GetHashCode(), Controls.Count - 1);
					
				}
			}
		}

		int[] ExixstsPoints(Point temp)
		{
			Point[] pointsNearPictureBox = {
				new Point(temp.X, temp.Y),
				new Point(temp.X + _SizeOfSquare, temp.Y),
				new Point(temp.X - _SizeOfSquare, temp.Y),
				new Point(temp.X, temp.Y + _SizeOfSquare),
				new Point(temp.X, temp.Y - _SizeOfSquare),
			};
			
			List<Point> temp2 = new List<Point>();
			foreach (var el in ExistingBox) {
				foreach (var el2 in pointsNearPictureBox) {
					if (el.Value == el2)
						temp2.Add(el2);
				}
			}
			List<int> index = new List<int>();
			foreach (var el in ExistingBox) {
				foreach (var el2 in pointsNearPictureBox) {
					if (el.Value == el2) {
						index.Add(el.Key);
					}
				}
			}
			return index.ToArray();
			
		}

		int _WhichPictureWasClicked(int i)
		{
			foreach (var el in HachCode_OfBox) {
				if (i == el.Key)
					return el.Value;
			}
			return 0;
		}
		void MyClick(object sender, EventArgs e)
		{
			
			Point temp = new Point(0, 0);
			
			int index = _WhichPictureWasClicked(sender.GetHashCode());
			
			foreach (var el in ExistingBox) {
				if (index == el.Key)
					temp = el.Value;
			}
			
			int[] IndexSquare = ExixstsPoints(temp);
			
			foreach (var el in IndexSquare) {
				if (Controls[el].BackColor == Color.Lime) {
					Controls[el].BackColor = Color.Azure;
				} else {
					Controls[el].BackColor = Color.Lime;
				}
			}
		}

		void Button1Click(object sender, EventArgs e)
		{
			if (int.Parse(textBox1.Text) < 10) {
				_genmap(int.Parse(textBox1.Text));
			} else
				MessageBox.Show("N должно быть меньше 10");
		}
		void MainFormKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				Close();
			}
		}
	}
}
