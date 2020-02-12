//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lights_Out
{
	public partial class MainForm : Form
	{
		int score = 1;
		const string name = "Количество ходов: ";
		
		List<int> ind;
		Dictionary <int,Point> ExistingBox;
		Dictionary <int,int> HachCode_OfBox;
		int _Widht = 600, _Height = 600, _SizeOfSquare = 60;
		public MainForm()
		{
			InitializeComponent();
			
		}
		
		void StartGameButton(object sender, EventArgs e)
		{
			try {
				if (textBox2.Text == "")
					textBox2.Text = "0";
				if (int.Parse(textBox1.Text) <= 25 & int.Parse(textBox1.Text) > 3) {
					if (int.Parse(textBox2.Text) < (int.Parse(textBox1.Text) * int.Parse(textBox1.Text) + 1)) {
						_Genmap(int.Parse(textBox1.Text), int.Parse(textBox2.Text));
					} else {
						textBox2.Text = "";
						throw new Exception("Количество включенных ламп не может превышать N*N");
					}
				} else {
					textBox1.Text = "";
					throw new Exception("N должно быть больше 3 и меньше 26");
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
		
		void _Genmap(int N, int count)
		{	
			Controls.Clear();
			
			if (N >= 10) {
				if (N >= 15)
					_SizeOfSquare = 30;
				else
					_SizeOfSquare = 40;
			}
			
			//Размеры формы
			ClientSize = new Size((N) * _SizeOfSquare, (N) * _SizeOfSquare);
			
			MaximumSize = new Size(Width, Height);
			MinimumSize = new Size(Width, Height);
			
			//Размеры сетки
			_Widht = _SizeOfSquare * N;
			_Height = _SizeOfSquare * N;
			
			//Сетка
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
				if (_LightOn(temp))
					i--;
				ind.Add(temp);
			}
			
			//пикчюрБоксы
			
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
					
					if (!_LightOn(p))
						field.BackColor = Color.Transparent;
					else
						field.BackColor = Color.DeepSkyBlue;
					field.Click += (_MyClick);
					Controls.Add(field);
					
					ExistingBox.Add(Controls.Count - 1, new Point(i * _SizeOfSquare, j * _SizeOfSquare));
					HachCode_OfBox.Add(field.GetHashCode(), Controls.Count - 1);
				}
			}
		}

		

		void _MyClick(object sender, EventArgs e)
		{
			
			if (_YouWin()) {
				MessageBox.Show("You win", "Congratulations", MessageBoxButtons.OK);
				Close();
			}
			
			Text = name + score++;
			
			//индекс пикчюрбокса, который был нажат
			int indexOfPictureBox = _WhichPictureWasClicked(sender.GetHashCode());
			
			//координаты пикчюрбокса который был нажат
			Point locationOfPictureBox = new Point(0, 0);
			foreach (var el in ExistingBox) {
				if (indexOfPictureBox == el.Key)
					locationOfPictureBox = el.Value;
			}
			
			//вычисление индексов пикчюрбоксов рядом с нажатым пикчюрбоксом
			int[] IndexSquare = _ExistingPictureBoxs(locationOfPictureBox);
			
			//смена цвета
			foreach (var el in IndexSquare) {
				if (Controls[el].BackColor == Color.Transparent) {
					Controls[el].BackColor = Color.DeepSkyBlue;
				} else {
					Controls[el].BackColor = Color.Transparent;
				}
			}
		}
		
		int[] _ExistingPictureBoxs(Point temp)
		{
			
			//Координаты всех соседних боксов
			Point[] pointsNearPictureBox = {
				new Point(temp.X, temp.Y),
				new Point(temp.X + _SizeOfSquare, temp.Y),
				new Point(temp.X - _SizeOfSquare, temp.Y),
				new Point(temp.X, temp.Y + _SizeOfSquare),
				new Point(temp.X, temp.Y - _SizeOfSquare),
			};
			
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

		bool _YouWin()
		{
			foreach (var el in HachCode_OfBox) {
				if (Controls[el.Value].BackColor == Color.DeepSkyBlue)
					return false;
			}
			return true;
		}

		//
		bool _LightOn(int p)
		{
			foreach (int el in ind) {
				if (p == el)
					return true;
			}
			return false;
		}
		
		void FormKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				Close();
			}
		}
	}
}
