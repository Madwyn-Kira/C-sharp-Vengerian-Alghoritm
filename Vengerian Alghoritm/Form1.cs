using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vengerian_Alghoritm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<int> PInt = new List<int>();
        List<int> VInt = new List<int>();
        List<List<int>> VVInt = new List<List<int>>();
        List<List<int>> VPInt = new List<List<int>>();

        private void Form1_Load(object sender, EventArgs e)
        {
            int taskCount = 4;
            int emplCount = 4;

            VVInt = new List<List<int>>(){new List<int>() {7,3,6,9,5},
                                          new List<int>() {7,5,7,5,6},
                                          new List<int>() {7,6,8,8,9},
                                          new List<int>() {3,1,6,5,7},
                                          new List<int>() {2,4,9,9,5}
            };

            #region Если на максимум
            List<int> maxStrVals = VVInt.Select(str => str.Max()).ToList();
            for (int i = 0; i < VVInt.Count; i++)
                for (int j = 0; j < (VVInt.Sum(x => x.Count) / VVInt.Count); j++)
                    VVInt[i][j] = maxStrVals[i] - VVInt[i][j];
            #endregion

            VVInt = hungarian(VVInt);

            foreach (List<int> i in VVInt)
            {
                foreach (int j in i)
                    richTextBox1.Text += j.ToString() + " \t ";

                richTextBox1.Text += "\r\n";
            }
            //MessageBox.Show("");
        }

        /// <summary>
        /// Венгерский алгоритм
        /// </summary>
        /// <param name="matrix">Матрица</param>
        /// <returns></returns>
        #region Алгоритм
        List<List<int>> hungarian(List<List<int>> matrix)
        {
            try
            {
                // Размеры матрицы
                int height = matrix.Count, width = matrix.Sum(x => x.Count) / height;

                // Значения, вычитаемые из строк (u) и столбцов (v)
                // VInt u(height, 0), v(width, 0);
                List<int> u = new List<int>(height);
                List<int> v = new List<int>(width);

                for (int a = 0; a < height; a++)
                    u.Add(0);
                for (int a = 0; a < width; a++)
                    v.Add(0);


                // Индекс помеченной клетки в каждом столбце
                List<int> markIndices = new List<int>(width);
                for (int a = 0; a < width; a++)
                    markIndices.Add(-1);

                // Будем добавлять строки матрицы одну за другой
                int count = 0;
                for (int i = 0; i < height; i++)
                {
                    List<int> links = new List<int>(width);
                    List<int> mins = new List<int>(width);
                    List<int> visited = new List<int>(width);

                    for (int a = 0; a < width; a++)
                    {
                        links.Add(-1);
                        mins.Add(int.MaxValue);
                        visited.Add(0);
                    }

                    // Разрешение коллизий (создание "чередующейся цепочки" из нулевых элементов)
                    int markedI = i, markedJ = -1, j = 0;
                    while (markedI != -1)
                    {
                        // Обновим информацию о минимумах в посещенных строках непосещенных столбцов
                        // Заодно поместим в j индекс непосещенного столбца с самым маленьким из них
                        j = -1;
                        for (int j1 = 0; j1 < width; j1++)
                            if (visited[j1] != 1)
                            {
                                if (matrix[markedI][j1] - u[markedI] - v[j1] < mins[j1])
                                {
                                    mins[j1] = matrix[markedI][j1] - u[markedI] - v[j1];
                                    links[j1] = markedJ;
                                }
                                if (j == -1 || mins[j1] < mins[j])
                                    j = j1;
                            }

                        // Теперь нас интересует элемент с индексами (markIndices[links[j]], j)
                        // Произведем манипуляции со строками и столбцами так, чтобы он обнулился
                        int delta = mins[j];
                        for (int j1 = 0; j1 < width; j1++)
                            if (visited[j1] == 1)
                            {
                                u[markIndices[j1]] += delta;
                                v[j1] -= delta;
                            }
                            else
                            {
                                mins[j1] -= delta;
                            }
                        u[i] += delta;

                        // Если коллизия не разрешена - перейдем к следующей итерации
                        visited[j] = 1;
                        markedJ = j;
                        markedI = markIndices[j];
                        count++;
                    }

                    // Пройдем по найденной чередующейся цепочке клеток, снимем отметки с
                    // отмеченных клеток и поставим отметки на неотмеченные
                    for (; links[j] != -1; j = links[j])
                        markIndices[j] = markIndices[links[j]];
                    markIndices[j] = i;
                }

                // Вернем результат в естественной форме
                List<List<int>> result = new List<List<int>>();
                for (int j = 0; j < width; j++)
                    if (markIndices[j] != -1)
                        result.Add(new List<int>() { markIndices[j], j });
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<List<int>>();
            }

        }
        #endregion

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
