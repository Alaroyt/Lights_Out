//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lights_Out
{
	public partial class MainForm : Form
	{
		List<int> ind;
		Dictionary <int,Point> ExistingBox;
		Dictionary <int,int> HachCode_OfBox;
		int _Widht = 600, _Height = 600, _SizeOfSquare = 60;
		public MainForm()
		{
			InitializeComponent();
			
		}

		bool LightOn(int p)
		{
			foreach (int el in ind) {
				if (p == el)
					return true;
			}
			return false;
		}
		void _genmap(int N, int count)
		{	
			if (N >= 10) {
				if (N >= 15)
					_SizeOfSquare=30;
				else
					_SizeOfSquare = 40;
			}
			
			MaximumSize = new Size(((N + 1) * _SizeOfSquare) - _SizeOfSquare / 2, ((1 + N) * _SizeOfSquare));
			MinimumSize = new Size(((N + 1) * _SizeOfSquare) - _SizeOfSquare / 2, ((1 + N) * _SizeOfSquare));
			
			
			Controls.Clear();
			
			_Widht = _SizeOfSquare * N;
			_Height = _SizeOfSquare * N;
			
			Height = (int)(_Height * 1.3);
			Width = (int)(_Widht * 1.2);
			
			//Поля
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
			
			//Рандомные точки
			ind = new List<int>();
			Random rnd = new Random();
			for (int i = 1; i <= count; i++) {
				int temp = rnd.Next(0, N * N);
				if (LightOn(temp))
					i--;
				ind.Add(temp);
			}
			
			//Создание пикчюрБоксов
			
			//ХэшКод всех пикчюрбоксов (нужно чтобы вычислять на какой нажали) (<Код><Номер>)
			HachCode_OfBox = new Dictionary<int,int>();
			//Координаты всех пикчюрБоксов (<Номер><Координаты>)
			ExistingBox = new Dictionary<int, Point>();
			var ExistingBox2 = new Dictionary<int, Point>();
			
			for (int p = 1, i = 0; i < N; i++) {
				for (int j = 0; j < N; p++, j++) {
					
					PictureBox field = new PictureBox();
					
					field.Size = new Size(_SizeOfSquare, _SizeOfSquare);
					field.Location = new Point(i * _SizeOfSquare, j * _SizeOfSquare);
					
					if (!LightOn(p))
						field.BackColor = Color.Lime;
					else
						field.BackColor = Color.Azure;
					field.Click += (MyClick);
					Controls.Add(field);
					
					ExistingBox.Add(Controls.Count - 1, new Point(i * _SizeOfSquare, j * _SizeOfSquare));
					HachCode_OfBox.Add(field.GetHashCode(), Controls.Count - 1);
				}
			}
		}

		int[] ExistingPictureBoxs(Point temp)
		{
			//Координаты всех соседних боксов
			Point[] pointsNearPictureBox = {
				new Point(temp.X, temp.Y),
				new Point(temp.X + _SizeOfSquare, temp.Y),
				new Point(temp.X - _SizeOfSquare, temp.Y),
				new Point(temp.X, temp.Y + _SizeOfSquare),
				new Point(temp.X, temp.Y - _SizeOfSquare),
			};
			
			//Проверка на реальность соседних боксов (не выходят ли координаты за пределы игрового поля)
//			List<Point> exploredPoints = new List<Point>();
//			foreach (var el in ExistingBox) {
//				foreach (var el2 in pointsNearPictureBox) {
//					if (el.Value == el2)
//						exploredPoints.Add(el2);
//				}
//			}
			
			//Индексы боксов, цвет которых будет изменен
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
			//индекс текстбокса, который был нажат
			int indexOfPictureBox = _WhichPictureWasClicked(sender.GetHashCode());
			
			//координаты текстбокса который был нажат
			Point locationOfPictureBox = new Point(0, 0);
			foreach (var el in ExistingBox) {
				if (indexOfPictureBox == el.Key)
					locationOfPictureBox = el.Value;
			}
			
			//вычисление индексов текстбоксов рядом с нажатым текстбоксом
			int[] IndexSquare = ExistingPictureBoxs(locationOfPictureBox);
			
			//смена цвета
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
			try {
				if (textBox2.Text == "")
					textBox2.Text = "0";
				if (int.Parse(textBox1.Text) <= 25 & int.Parse(textBox1.Text) > 3) {
					if (int.Parse(textBox2.Text) < (int.Parse(textBox1.Text) * int.Parse(textBox1.Text) + 1)) {
						_genmap(int.Parse(textBox1.Text), int.Parse(textBox2.Text));
					} else {
						textBox2.Text = "";
						throw new Exception("Количество включенных ламп не может превышать N*N");
					}
				} else {
					textBox1.Text = "";
					throw new Exception("N должно быть больше 3 и меньше 25");
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
		void MainFormKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				Close();
			}
		}
	}
}
