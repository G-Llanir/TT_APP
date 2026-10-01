using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace TT_Hub.FrameWork
{

        #region | Funções para GRID
        public class Grid
        {
            /// <summary>
            /// Função para Esconder as colunas de um DataGridView
            /// </summary>
            /// <param name="e">GridView</param>
            /// <param name="nCol1">Número da Coluna </param>
            public static void EsconderColunas(GridViewRowEventArgs e, int nCol1)
            {
                EsconderColunas(e, nCol1, -1, -1, -1, -1, -1);
            }
            public static void EsconderColunas(GridViewRowEventArgs e, int nCol1, int nCol2)
            {
                EsconderColunas(e, nCol1, nCol2, -1, -1, -1, -1);
            }
            public static void EsconderColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3)
            {
                EsconderColunas(e, nCol1, nCol2, nCol3, -1, -1, -1);
            }
            public static void EsconderColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3, int nCol4)
            {
                EsconderColunas(e, nCol1, nCol2, nCol3, nCol4, -1, -1);
            }
            public static void EsconderColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3, int nCol4, int nCol5)
            {
                EsconderColunas(e, nCol1, nCol2, nCol3, nCol4, nCol5, -1);
            }
            public static void EsconderColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3, int nCol4, int nCol5, int nCol6)
            {
                if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
                {
                    if (nCol1 != -1) e.Row.Cells[nCol1].Visible = false;
                    if (nCol2 != -1) e.Row.Cells[nCol2].Visible = false;
                    if (nCol3 != -1) e.Row.Cells[nCol3].Visible = false;
                    if (nCol4 != -1) e.Row.Cells[nCol4].Visible = false;
                    if (nCol5 != -1) e.Row.Cells[nCol5].Visible = false;
                    if (nCol6 != -1) e.Row.Cells[nCol6].Visible = false;
                }

            }


            /// <summary>
            /// Função para Mostrar as colunas de um DataGridView
            /// </summary>
            /// <param name="e">GridView</param>
            /// <param name="nCol1">Número da Coluna </param>
            public static void MostrarColunas(GridViewRowEventArgs e, int nCol1)
            {
                MostrarColunas(e, nCol1, -1, -1, -1, -1, -1);
            }
            public static void MostrarColunas(GridViewRowEventArgs e, int nCol1, int nCol2)
            {
                MostrarColunas(e, nCol1, nCol2, -1, -1, -1, -1);
            }
            public static void MostrarColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3)
            {
                MostrarColunas(e, nCol1, nCol2, nCol3, -1, -1, -1);
            }
            public static void MostrarColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3, int nCol4)
            {
                MostrarColunas(e, nCol1, nCol2, nCol3, nCol4, -1, -1);
            }
            public static void MostrarColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3, int nCol4, int nCol5)
            {
                MostrarColunas(e, nCol1, nCol2, nCol3, nCol4, nCol5, -1);
            }
            public static void MostrarColunas(GridViewRowEventArgs e, int nCol1, int nCol2, int nCol3, int nCol4, int nCol5, int nCol6)
            {
                if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
                {
                    if (nCol1 != -1) e.Row.Cells[nCol1].Visible = true;
                    if (nCol2 != -1) e.Row.Cells[nCol2].Visible = true;
                    if (nCol3 != -1) e.Row.Cells[nCol3].Visible = true;
                    if (nCol4 != -1) e.Row.Cells[nCol4].Visible = true;
                    if (nCol5 != -1) e.Row.Cells[nCol5].Visible = true;
                    if (nCol6 != -1) e.Row.Cells[nCol6].Visible = true;
                }

            }

            /// <summary>
            /// Função para Somar e Formatar Colunas de Valores
            /// </summary>
            /// <param name="gv">GridView</param>
            /// <param name="MostraRotulo">Mostra totalização de Linhas</param>
            /// <param name="TipoFormatacao">Tipo da Formatação da Coluna 1=Inteiro, 2=Moeda, 3=Peso</param>
            public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, int nCol)
            {
                SomarColunas(gv, MostraRotulo, TipoFormatacao, nCol, -1);
            }
            public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, int nCol, int nCol2)
            {
                SomarColunas(gv, MostraRotulo, TipoFormatacao, nCol, nCol2, -1);
            }
            public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, int nCol, int nCol2, int nCol3)
            {
                SomarColunas(gv, MostraRotulo, TipoFormatacao, nCol, nCol2, nCol3, -1);
            }
            public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, int nCol, int nCol2, int nCol3, int nCol4)
            {
                SomarColunas(gv, MostraRotulo, TipoFormatacao, nCol, nCol2, nCol3, nCol4, -1);
            }
            public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, int nCol, int nCol2, int nCol3, int nCol4, int nCol5)
            {
                SomarColunas(gv, MostraRotulo, TipoFormatacao, nCol, nCol2, nCol3, nCol4, nCol5, -1);
            }
            public static void SomarColunas(GridView gv, bool MostraRotulo, Formatação TipoFormatacao, int nCol, int nCol2, int nCol3, int nCol4, int nCol5, int nCol6)
            {

                string sTpFormatacao = "";
                string sComplemento = "";
                double nTotal1 = 0;
                double nTotal2 = 0;
                double nTotal3 = 0;
                double nTotal4 = 0;
                double nTotal5 = 0;
                double nTotal6 = 0;

                double nValor1 = 0;
                double nValor2 = 0;
                double nValor3 = 0;
                double nValor4 = 0;
                double nValor5 = 0;
                double nValor6 = 0;



                switch (TipoFormatacao)
                {
                    case Formatação.Inteiro:
                        sTpFormatacao = "{0:n0}";
                        break;
                    case Formatação.Moeda:
                        sTpFormatacao = "{0:C2}";
                        break;
                    case Formatação.Numero:
                        sTpFormatacao = "{0:n2}";
                        break;
                    case Formatação.Peso:
                        sTpFormatacao = "{0:n3}";
                        // sComplemento = " Kg";
                        break;
                }

                //  try
                {

                    for (int i = 0; i < gv.Rows.Count; i++)
                    {
                        {

                            if (nCol != -1)
                            {
                                bool bAtualizaColuna1 = true;
                                nValor1 = Grid.RecuperaValorColuna(gv.Rows[i].Cells[nCol]);
                                try
                                {
                                    if (gv.Rows[i].Cells[nCol].Controls[0].GetType().Name == "HyperLink")
                                    {
                                        if (nValor1 == 0)
                                        {
                                            bAtualizaColuna1 = true;
                                        }
                                        else
                                        {
                                            bAtualizaColuna1 = false;
                                        }
                                    }
                                    else
                                    {
                                        bAtualizaColuna1 = true;
                                    }

                                }
                                catch
                                {
                                    bAtualizaColuna1 = true;
                                }

                                if (bAtualizaColuna1)
                                {
                                    gv.Rows[i].Cells[nCol].Text = string.Format(sTpFormatacao, nValor1) + sComplemento;
                                    gv.Rows[i].Cells[nCol].HorizontalAlign = HorizontalAlign.Right;
                                }

                            }

                            if (nCol2 != -1)
                            {
                                nValor2 = Grid.RecuperaValorColuna(gv.Rows[i].Cells[nCol2]);
                                gv.Rows[i].Cells[nCol2].Text = string.Format(sTpFormatacao, nValor2) + sComplemento;
                                gv.Rows[i].Cells[nCol2].HorizontalAlign = HorizontalAlign.Right;
                            }
                            if (nCol3 != -1)
                            {
                                nValor3 = Grid.RecuperaValorColuna(gv.Rows[i].Cells[nCol3]);
                                gv.Rows[i].Cells[nCol3].Text = string.Format(sTpFormatacao, nValor3) + sComplemento;
                                gv.Rows[i].Cells[nCol3].HorizontalAlign = HorizontalAlign.Right;
                            }
                            if (nCol4 != -1)
                            {
                                nValor4 = Grid.RecuperaValorColuna(gv.Rows[i].Cells[nCol4]);
                                gv.Rows[i].Cells[nCol4].Text = string.Format(sTpFormatacao, nValor4) + sComplemento;
                                gv.Rows[i].Cells[nCol4].HorizontalAlign = HorizontalAlign.Right;
                            }
                            if (nCol5 != -1)
                            {
                                nValor5 = Grid.RecuperaValorColuna(gv.Rows[i].Cells[nCol5]);
                                gv.Rows[i].Cells[nCol5].Text = string.Format(sTpFormatacao, nValor5) + sComplemento;
                                gv.Rows[i].Cells[nCol5].HorizontalAlign = HorizontalAlign.Right;
                            }
                            if (nCol6 != -1)
                            {
                                nValor6 = Grid.RecuperaValorColuna(gv.Rows[i].Cells[nCol6]);
                                gv.Rows[i].Cells[nCol6].Text = string.Format(sTpFormatacao, nValor6) + sComplemento;
                                gv.Rows[i].Cells[nCol6].HorizontalAlign = HorizontalAlign.Right;
                            }




                            nTotal1 += nValor1;
                            nTotal2 += nValor2;
                            nTotal3 += nValor3;
                            nTotal4 += nValor4;
                            nTotal5 += nValor5;
                            nTotal6 += nValor6;
                        }
                    }

                    gv.FooterRow.Font.Bold = true;
                    if (MostraRotulo)
                    {
                        int nColunaTotalizacao = 0;
                        if (gv.FooterRow.Cells[0].Visible == false)
                        {
                            nColunaTotalizacao = 1;
                        }

                        gv.FooterRow.Cells[nColunaTotalizacao].Text = "Total: " + gv.Rows.Count.ToString();
                        gv.FooterRow.Cells[nColunaTotalizacao].HorizontalAlign = HorizontalAlign.Left;
                    }

                    // Imprimindo os Totais
                    if (nCol != -1)
                    {
                        gv.FooterRow.Cells[nCol].Text = string.Format(sTpFormatacao, nTotal1) + sComplemento;
                        gv.FooterRow.Cells[nCol].HorizontalAlign = HorizontalAlign.Right;
                    }
                    if (nCol2 != -1)
                    {
                        gv.FooterRow.Cells[nCol2].Text = string.Format(sTpFormatacao, nTotal2) + sComplemento;
                        gv.FooterRow.Cells[nCol2].HorizontalAlign = HorizontalAlign.Right;
                    }
                    if (nCol3 != -1)
                    {
                        gv.FooterRow.Cells[nCol3].Text = string.Format(sTpFormatacao, nTotal3) + sComplemento;
                        gv.FooterRow.Cells[nCol3].HorizontalAlign = HorizontalAlign.Right;
                    }
                    if (nCol4 != -1)
                    {
                        gv.FooterRow.Cells[nCol4].Text = string.Format(sTpFormatacao, nTotal4) + sComplemento;
                        gv.FooterRow.Cells[nCol4].HorizontalAlign = HorizontalAlign.Right;
                    }
                    if (nCol5 != -1)
                    {
                        gv.FooterRow.Cells[nCol5].Text = string.Format(sTpFormatacao, nTotal5) + sComplemento;
                        gv.FooterRow.Cells[nCol5].HorizontalAlign = HorizontalAlign.Right;
                    }
                    if (nCol4 != -1)
                    {
                        gv.FooterRow.Cells[nCol6].Text = string.Format(sTpFormatacao, nTotal6) + sComplemento;
                        gv.FooterRow.Cells[nCol6].HorizontalAlign = HorizontalAlign.Right;
                    }



                }
                //catch
                {


                }

            }
            static double RecuperaValorColuna(System.Web.UI.WebControls.TableCell celula)
            {
                string sTipoCelula = "";
                string sValor = "";
                double nValor = 0;

                try
                {

                    if (celula.Controls.Count > 0)
                    {
                        sTipoCelula = celula.Controls[0].ToString().Trim();

                        if (sTipoCelula == "System.Web.UI.WebControls.HyperLink")
                        {
                            System.Web.UI.WebControls.HyperLink hyperlink = (System.Web.UI.WebControls.HyperLink)celula.Controls[0];
                            sValor = hyperlink.Text.ToString();
                        }
                        else
                        {
                            sValor = celula.Text.ToString();

                        }
                    }
                    else
                    {
                        sValor = celula.Text.ToString();
                    }
                }
                catch
                {
                    sValor = celula.Text.ToString();

                }

                try
                {
                    nValor = Convert.ToDouble(sValor.Trim().Replace("R$", ""));
                }
                catch
                {
                    //Transfolha.Erro(sValor);
                }

                return nValor;
            }


            #region | Enumeradores


            public enum Formatação
            {
                Inteiro = 1,
                Moeda = 2,
                Peso = 3,
                Numero = 4

            }

            #endregion
            #endregion
        }


}