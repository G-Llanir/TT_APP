public static class Permissao
{
	public static int AlterarBancos = 587;
    public static class Atividades // 498
    {
        public static class Projetos
        {
            public static int Consultar = 500;
            public static int Incluir = 501;
            public static int Alterar = 502;
            public static int AssociarUsuarios = 538;
            public static int Nivel_Gestor = 555;
            public static int Nivel_Diretor = 556;											 
	    }
    }

    public static class Status // 32
    {
        public static int Consultar = 33;
        public static int Incluir = 34;
        public static int Alterar = 35;
    }

    public static class Fluxo // 36
    {
        public static int Consultar = 37;
        public static int Incluir = 38;
        public static int Alterar = 39;
    }

    public static class CondicaodePagamento // 40
    {
        public static int Consultar = 41;
        public static int Incluir = 42;
        public static int Alterar = 43;
    }

    public static class Motivos // 44
    {
        public static int Consultar = 45;
        public static int Incluir = 46;
        public static int Alterar = 47;
    }

    public static class Pedidos // 6
    {
        public static int Consultar = 7;
        public static int Incluir = 8;
        public static int Alterar = 9;
        public static int CancelarPedido = 10;
        public static int EnxergarValores = 29;
        public static int AlterarPrevisaoEntrega = 30;
        public static int AlterarEstimativaEntrega = 199;
        public static int AlterarStatusPedido = 31;
        public static int ExibirComissionador_Comissao = 75;
        public static int AlterarDepartamento = 128;
        public static int ExcluirTarefas = 132;
        public static int ExcluirArquivos = 198;
        public static int LM_Engenharia = 203;
        public static int LM_Obras = 204;
        public static int Visualizar_Aba_Resultado = 308;
        public static int Visualizar_Aba_De_Para = 335;
        public static int PaginaResultado = 353;
		public static int Visualizar_Aba_Documentos = 433;
		public static int FinalizarMultiplasTarefas = 434;
		public static int Visualizar_Aba_STSO = 443;
		public static int SolicitarDocumentosSTSO = 449;										  
		public static int Visualizar_Aba_ART = 450;
        public static int GerarCentroDeCusto = 546;
		public static int Visualizar_Aba_Faturamento = 565;																	
		public static int GerarMedicao = 572;	
        public static int Visualizar_TTL_TTS__Dashboard = 592;
		public static int Visualizar_FluxosSeparados__Dashboard = 598;
        public static int Download_ArquivoMorto = 242;
        public static int Visualizar_Aba_ArquivoMorto = 200;
    }

    public static class Empresas // 354
    {
        public static int PaginaEmpresas = 355;
        public static int IncluirEmpresas = 360;
        public static int AlterarEmpresas = 361;
        public static int ArquivoDocumento = 375;
        public static int ArquivoSTSO = 376;
        public static int ArquivoSocios = 378;
        public static int ArquivoCertidõesEmpresa = 379;
        public static int ArquivoCertidõesSocios = 380;
        public static int ArquivoBalancos = 381;
        public static int ArquivoSeguros = 382;
		public static int Aba_STSO = 441;								 
    }

    public static class Certificados // 350
    {
        public static int PaginaCertificado = 351;
        public static int IncluirCertificado = 362;
        public static int AlterarCertificado = 363;
    }

    public static class Segmentos // 396
    {
        public static int PaginaSegmentos = 399;
        public static int IncluirSegmentos = 397;
        public static int AlterarSegmentos = 398;
    }

    public static class TipoProdutos // 107
    {
        public static int Consultar = 108;
        public static int Incluir = 109;
        public static int Alterar = 110;
    }

    public static class Produtos // 329
    {
        public static int Consultar = 20;
        public static int ConsultarAbaArquivos = 213;
        public static int ConsultarAbaComopsicao = 214;
        public static int ConsultarAbaMovimentacao = 215;
        public static int ConsultarAbaSugestao = 216;
        public static int ConsultarAbaFornecedores = 217;
        public static int Incluir = 21;
        public static int Alterar = 22;
        public static int ConsultarSaldos = 140;
        public static int ImportarSaldos = 141;
        public static int ConsultarAbaIdiomas = 1010;
		public static int VisualizarExibeComercial_LMELMO = 426;														
		public static int AlterarFatorGlobal = 442;			
        public static int BloquearEdicao = 447;
        public static int ConsultarAbaFabricacao = 491;
        public static int ConsultarAbaTabelas = 503;
        public static int EditarAbaTabelas = 504;		
		public static int ConsultaProdutosPendenteValidacao = 566;													
    }

    public class Servicos // 319
    {
        public static int Consultar = 316;
        public static int Incluir = 317;
        public static int Alterar = 318;
    }

    public static class Sub_Servicos // 415
    {
        public static int Consultar = 393;
        public static int Incluir = 417;
        public static int Alterar = 416;

    }

    public static class Produtos_Recursos // 331
    {
        public static int Consultar = 332;
        public static int Incluir = 333;
        public static int Alterar = 334;
    }

    public static class Parceiros // 15
    {
        public static int Consultar = 16;
        public static int Incluir = 17;
        public static int Alterar = 18;
        public static int EnxergarClientesVisualizacaoDiferenciada = 193;
		public static int AlterarFormaPagamento = 494;											  
		public static int VisualizarAbaFinanceiro = 582;													
		public static int BloquearEdicao = 606;									   
    }

    public static class Departamentos // 23
    {
        public static int Consultar = 24;
        public static int Incluir = 25;
        public static int Alterar = 26;
    }

    public static class TipoArquivo // 471
    {
        public static int Consultar = 472;
        public static int Incluir = 473;
        public static int Alterar = 474;
    }
    
    public static class Tarefas // 48
    {
        public static int Consultar = 50;
        public static int Incluir = 51;
        public static int Alterar = 62;
    }


    public static class Mensagens // 675
    {
        public static int Consultar = 676;
        public static int Enviar = 677;
    }


    public static class IA // 683
    {
        public static int Consultar = 684;
        public static int ExecutarAcoes = 685;
        public static int ExecutarAcoesCriticas = 686;
        public static int VisualizarAuditoria = 687;
        public static int AdministrarFerramentas = 688;
        public static int VisualizarArquivos = 689;
        public static int VisualizarUso = 693;
        public static int BaseConhecimento = 697;
        public static int UsarConhecimento = 698;
		public static int AdministrarWorkflows = 699;																						
    }
    public static class Recursos // 77
    {
        public static int Consultar = 78;
        public static int Incluir = 79;
        public static int Alterar = 80;
    }

    public static class Familia // 97
    {
        public static int Consultar = 98;
        public static int Incluir = 99;
        public static int Alterar = 100;
    }

    public static class NCM // 101
    {
        public static int Consultar = 102;
        public static int Incluir = 103;
        public static int Alterar = 104;
    }

    public static class CEST // 111
    {
        public static int Consultar = 112;
        public static int Incluir = 113;
        public static int Alterar = 114;
    }

    public static class RelatorioDespesas // 456
    {
        public static int Consultar = 457;
        public static int SalvarDespesa = 643;
        public static int GerenciarPrevisões = 644;
        public static int GerenciarLancamentos = 645;
        public static int GerenciarStatus = 646;
        public static int ExibirRelatorio = 647;
        public static int NovoRelatorio = 648;
		public static int Reembolso = 654;								  
    }	 

    public static class OrdemServico // 171
    {
        public static int Consultar = 172;
        public static int Incluir = 173;
        public static int Alterar = 174;

        public static class TiposDeProcedimentos // 117
        {
            public static int Consultar = 118;
            public static int Incluir = 119;
            public static int Alterar = 120;
        }

        public static class Procedimentos // 121
        {
            public static int Consultar = 122;
            public static int Incluir = 123;
            public static int Alterar = 124;
        }
    }

    public static class WMS // 87
    {
        public static class OPI // 303
        {
            public static int Excluir = 304;
            public static int Alterar = 305;
            public static int Consultar = 306;
			public static int GerarOPI = 540;								 
		}

        public static class Movimentacao // 
        {
            public static int ImportarXML = 653;
        }
    
        public static class Cadastro_Unidades // 403
        {
            public static int Consultar = 404;
            public static int Incluir = 405;
            public static int Alterar = 406;
        }

        public static class RelatorioMovimentacao // 484
        {
            public static int Consultar = 485;
        }

        public static class SeparacaoPedidos // 495
        {
            public static int Separacao = 496;
        }

        public static class Sistemas // 550
        {
            public static int Visualizar = 551;
        }


        public static class Rastreabilidade // 667
        {
            public static int Consultar = 667;
										   
												   
        } 

        public static class ConsultarSaldo // 139
        {
            public static int Consultar = 140;
            public static int Editar = 665;
            public static int ImportarSaldos = 141;
        }
        public static class Consultar_LM // 691
        {
            public static int ConsultarLM = 692;
        }										   
    }

    public static class RRHH // 81
    {
        public static int Consultar = 83;
        public static int Incluir = 84;
        public static int Alterar = 85;
        public static int ConsultarDadosColaborador = 86;
        public static int ExibirDocumentos_RH = 125;
        public static int SSTT = 126;
        public static int ExibirDocumentos_SSTT = 127;
        public static int Ocorrencias = 129;
        public static int ExibirDocumentos_Ocorrencias = 130;
        public static int ExibirAbaBeneficios = 166;
        public static int ExibirDadosPessoais = 165;
        public static int ExibirAbaAvaliacao = 202;
        public static int AssociarUsuario = 201;
		public static int AbaArquivoMorto = 497;


        public static class Planos
        {
            public static int Consultar = 602;
            public static int Incluir = 603;        
            public static int Excluir = 604;
            public static int Alterar = 605;
        }

        public static class         OcorrenciasElogios //593;
        {
            public static int Consultar = 594;
            public static int Incluir = 599;
            public static int Alterar = 600;
        }

        public static class Entrega_EPI // 418
        {
            public static int Consultar = 419;
            public static int Incluir = 421;
            public static int Alterar = 420;
			public static int ConfirmarEntrega = 562;
			public static int Consultar_Controle_de_EPIs = 571;										  
			public static int AlterarPeriodicidade = 631;											 
        }

        public static class FuncoesCarteira // 143
        {
            public static int Consultar = 144;
            public static int Incluir = 145;
            public static int Alterar = 146;
			public static int ConsultarDadosNívelII = 462;
            public static int Cadeado = 564;
        }

        public static class FuncoesConversas // 231
        {
            public static int Consultar = 234;
            public static int Incluir = 232;
            public static int Alterar = 233;
            public static int Excluir = 235;
        }

        public static class FuncoesAbaOcorrencias // 248
        {
            public static int Bloco_Ausencia = 248;
            public static int Bloco_Eventos = 249;
            public static int Bloco_Veiculos = 250;
        }

        public static class Avaliacao // 477
        {
            public static int Consultar = 467;
            public static int Incluir = 468;
            public static int VisualizarAvaliacoesCadastradas = 469;

            public static class Resultado // 478
            {
                public static int Consultar = 479;
                public static int VisualizarASU = 480;
                public static int VisualizarASR = 481;
                public static int VisualizarASD = 482;
                public static int VisualizarResultado = 483;
            }
        }

        public static class RelatorioBeneficios // 262
        {
            public static int Consultar = 262;
        }

        public static class DashboardRRHH // 595
        {
            public static int Consultar = 314;
            public static int VisualizaAniversario = 596;
        }

        public static class GHE // 573
        {
            public static int Consultar = 574;
            public static int Incluir = 575;

        }
		
		
        public static class Setor // 579
        {
            public static int Consultar = 580;
            public static int Incluir = 581;

        public static class OcorrenciasElogios // 593
        {
            public static int Consultar = 594;
            public static int Incluir = 599;
            public static int Alterar = 600;   											
        }
        }

        public static class Manutencao_NR
        {
            public static int Consultar = 608;
            public static int Editar = 609;
        }

        public static class Seguro
        {
            public static int Consultar = 615;
            public static int Editar = 616;
            public static int Incluir = 617;
        }

        public static class RelatorioSeguro // 618
        {
            public static int Consultar = 619;
            public static int GerarRelatorio = 620;            
        }
		
		public static class TerceirosPJ
		{
			public static int Terceiro = 622;
			public static int Consultar = 623;
			public static int GerarRelatorio = 624;
			public static int GerarAjuste = 625;
			public static int GerarFechamento = 626;
			public static int EditarAjuste = 627;
			public static int COntroleHorasTFlow = 628;
		}


        public static class PagamentosColaboradores // 678
        {
            public static int Consultar = 679;
            public static int Incluir = 680;
            public static int Alterar = 681;
            public static int GerarContasPagar = 682;
        }														  

		public static class PlanoSaude // 649
        {
            public static int Consultar = 650;
            public static int Incluir = 651;
            public static int Editar = 652;
        }

        public static class ControleFerias // 658
        {
            public static int Consultar = 659;
            public static int Incluir = 660;
            public static int Editar = 661;
        }												 
    }

    public static class Patrimonio // 179
    {
        public static class Grupos // 180
        {
            public static int Consultar = 181;
            public static int Incluir = 182;
            public static int Alterar = 183;
        }

        public static class Local // 184
        {
            public static int Consultar = 185;
            public static int Incluir = 186;
            public static int Alterar = 187;
        }

        public static class Categoria // 188
        {
            public static int Consultar = 189;
            public static int Incluir = 190;
            public static int Alterar = 191;
        }

        public static class ControlePatrimonio // 258
        {
            public static int Consultar = 259;
            public static int Incluir = 260;
            public static int Alterar = 261;
        }
    }

    public static class Usuarios // 54
    {
        public static int Consultar = 55;
        public static int Incluir = 56;
        public static int Alterar = 57;
        public static int Excluir = 76;
        public static int VerSenha = 315;
        public static int Editar_PaginaInicial = 578;
        public static int EfetuarLogin = 674;
    }

    public static class Veiculos // 133
    {
        public static int Consultar = 134;
        public static int Incluir = 135;
        public static int Alterar = 136;
    }

    public static class TI // 137
    {
        public static class Change_Log // 269
        {
            public static int Consultar = 453;
            public static int Aprovar = 178;
            public static int Incluir = 138;
            public static int Alterar = 454;
        }

        public class Controle_de_Permissoes // 265
        {
            public static int Consultar = 266;
            public static int Incluir = 267;
            public static int Alterar = 268;
        }
    }

    public static class Comercial // 105
    {
        public static class Dashboard_Comercial
        {
            public static int Consultar = 635;
            public static int Todos_Vendedores = 636;
        }
        public static class DashboardCRM // 436
        {
            public static int Consultar = 437;
            public static int Visualizar_Vendedor = 438;
            public static int Visualizar_Supervisor = 439;
            public static int Visualizar_Diretor = 440;
        }

        public static class Manutencao // 106
        {
            public static class TipoOrcamento // 321
            {
                public static int Consultar = 322;
                public static int Alterar = 323;
                public static int Incluir = 324;
            }

            public static class CategoriaEscopo // 368
            {
                public static int Consultar = 372;
                public static int Alterar = 373;
                public static int Incluir = 374;
            }
        }

        public static class Escopo // 367
        {
            public static int Consultar = 369;
            public static int Alterar = 370;
            public static int Incluir = 371;
        }

        public static class TabelaDePreco // 157
        {
            public static int Master = 297;
            public static int Visualizar_Tabelas_de_Controle_Nacional = 298; // Custo TT & Fab TT
            public static int Editar_Tabelas_de_Controle_Nacional = 299; // Custo TT & Fab TT
            public static int Visualizar_Vendas_Nacional = 300;
            public static int Visualizar_Fornecedor_Nacional = 301;
            public static int Editar_Fornecedor_Nacional = 302;
            public static int Visualizar_Tabelas_de_Controle_Internacional = 309; // Custo TT & Fab TT
            public static int Editar_Tabelas_de_Controle_Internacional = 310; // Custo TT & Fab TT
            public static int Visualizar_Vendas_Internacional = 311;
            public static int Visualizar_Fornecedor_Internacional = 312;
            public static int Editar_Fornecedor_Internacional = 313;
            public static int Consultar = 158;
            public static int Incluir = 159;
            public static int Alterar = 160;
			public static int BloquearEdicao = 448;
			public static int Editar_LPU = 451;			
			public static int Visualizar_LPU = 452;
        }

        public static class CategoriaVendas // 167
        {
            public static int Consultar = 168;
            public static int Incluir = 169;
            public static int Alterar = 170;
        }

        public static class Cotacoes // 253
        {
            public static int Consultar = 255;
            public static int Incluir = 254;
            public static int Alterar = 256;
        }

        public static class Orcamento // 325
        {
            public static int Consultar = 326;
            public static int Incluir = 327;
            public static int Alterar = 328;
            public static int Nivel_1 = 356;
            public static int Nivel_2 = 357;
            public static int Nivel_3 = 358;
			public static int Nivel_Adm = 690;								  
            public static int AlterarValorComDesconto = 428;
            public static int Alterar_ImagemFornecedores = 597;
        }

        public static class CRM // 336
        {
            public static int Consultar = 336;
            public static int Visualizar_Vendedores = 337;
            public static int Confidencial = 348;
            public static int Excluir = 366;
        }

        public static class Tarefas_STSO // 444
        {
            public static int STSO = 445;
            public static int Visualizar_Pedidos_STSO = 446;
        }				
        
        public static class PosVendas //637
        {
            public static int ConfirmacaoEntrega = 638 ;
        }
    }

    public static class Financeiro  // 203
    {
        public static class ContasReceber // 204
        {
            public static int Consultar = 205;
            public static int Incluir = 206;
            public static int AlterarVencimento = 207;
            public static int EfetuarBaixa = 208;
            public static int Excluir = 251;
            public static int Reabrir = 294;
            public static int Adiantar = 296;
            public static int VisualizarTudo = 344;
			public static int IncluiArquivo = 458;									  
        }
 
        public static class ContasPagar // 209
        {
            public static int Consultar = 210;
            public static int Incluir = 211;
            public static int EfetuarPagamento = 212;
            public static int Excluir = 252;
            public static int Reabrir = 293;
            public static int VisualizarTudo = 343;
            public static int Comparativo = 365;
			public static int IncluiArquivo = 461;  												  
        }

        public static class Provisao // 338
        {
            public static int Consultar = 341;
            public static int Incluir = 340;
            public static int Excluir = 342;
        }

        public static class DashboardAdm // 264
        {
            public static int VisualizarTudoPagar = 346;
            public static int VisualizarTudoReceber = 347;
        }

        public static class ExtratoBancario // 389
        {
            public static int Consultar = 390;
            public static int Importar = 391;
            public static int DownloadExtrato = 392;
        }

        public static class Financeiro_Cliente // 400
        {
            public static int Consultar = 401;
        }

        public static class Cartoes // 427
        {
            public static int Consultar = 429;
            public static int Inserir = 430;
            public static int VisualizarTudo = 431;
			public static int Editar = 455;
            public static int Excluir = 529;
			public static int EditarItem = 539;								   

            public static class Fatura // 464
            {
                public static int Consultar = 465;
            }
        }
		
        public static class RelatorioPagarReceber // 459
        {
            public static int Consultar = 460;
        }

        public static class Faturamento // 475
        {
            public static int Consultar = 476;
        }

        public static class ImportadorNFe // 486
        {
            public static int Consultar = 487;
        }

        public static class GerenciadorNFe // 489
        {
            public static int Consultar = 490;
        }

        public static class RelatorioFinanceiro // 629
        {
            public static int Consultar = 630;
        }

        public static class FluxoCaixa // 708
        {
            public static int Consultar = 709;
        }											 
    }

    public static class Administracao // 148
    {
        public static class ContasBancarias // 149
        {
            public static int Consultar = 150;
            public static int Incluir = 151;
            public static int Alterar = 152;
        }

        public static class CodigoContabil // 153
        {
            public static int Consultar = 154;
            public static int Incluir = 155;
            public static int Alterar = 156;
        }

        public static class CentrodeCustos // 161
        {
            public static int Consultar = 162;
            public static int Incluir = 163;
            public static int Alterar = 164;
            public static int Manutencao = 583;
            public static int TpCentroCusto = 584;
            public static int InserirTpCentroCusto = 585;
            public static int AlterarTpCentroCusto = 586;
        }

        public static class Moedas // 175
        {
            public static int Consultar = 176;
            public static int Alterar = 177;
        }

        public static class Meta // 276
        {
            public static int Consultar = 278;
            public static int Alterar = 277;
        }

        public static class CategoriaContasPagar // 279
        {
            public static int Consultar = 280;
            public static int Incluir = 281;
        }

        public static class AprovacaoPagamento // 282
        {
            public static int Consultar = 283;
            public static int Incluir = 284;
            public static int VisualizarTudo = 345;
        }

        public static class CategoriaContasReceber // 290
        {
            public static int Consultar = 291;
            public static int Incluir = 292;
        }

        public static class NFe // 295
        {
            public static int Consultar = 295;
        }

        public static class Fiscal // 383
        {
            public static class Regras // 384
            {
                public static int Consultar = 385;
                public static int Incluir = 386;
                public static int Alterar = 387;
                public static int Simular = 388;
            }


            public static class RegrasICMS
            {
                public static int Consultar = 640;
                public static int Incluir = 641;
                public static int Alterar = 642;
            }
        }

        public static class CFOP // 558
        {
            public static int Consultar = 559;
        }

        public static class CST_IBS_CBS // 694
        {
            public static int Consultar = 695;
            public static int Incluir = 696;
        }											  
       
	   public static class NBS // 710
        {
            public static int Consultar = 700;
            public static int Incluir = 701;
            public static int Alterar = 702;
        }

        public static class IndicadorOperacao // 703
        {
            public static int Consultar = 704;
            public static int Incluir = 705;
            public static int Alterar = 706;
        }
        public static class EmissaoNFe // 560
        {
            public static int Consultar = 561;
        }

        public static class Historico // 588
        {
            public static class ImportaçãodeXML // 589
            {
                public static int Consultar = 590;
                public static int incluir = 591;
            }
        }											
    }

    public static class Manutencao // 14
    {
        public class Feriados //223
        {
            public static int Consultar = 224;
            public static int Incluir = 225;
            public static int Alterar = 226;
        }

        public class Tipos_Gastos // 512
        {
            public static int Consultar = 513;
        }

        public class Codigo_Servico // 505
        {
            public static int Consultar = 506;
            public static int Inserir = 548;
            public static int Editar = 549;
        }

        public class ServicoMunicipal // 576
        {
            public static int Consultar = 577;
        }											
        
        public class DadosControleAcesso // 654
        {
            public static int Consultar = 655;
            public static int Alterar = 656;
            public static int Incluir = 657;
            public static int Visualiza_Aba_Historico = 662;
            public static int Alterar_Cadeado = 663;															
        }											   
	}

    public static class FAQ // 218
    {
        public static int Consultar = 219;
        public static int Incluir = 220;
        public static int Alterar = 221;
        public static int Aprovar = 222;
    }

    public static class Forum // 227
    {
        public static int Consultar = 228;
        public static int Incluir = 229;
        public static int Alterar = 230;
    }

    public static class Comex // 194
    {
        public static int Resultado = 353;
        public static int VisualizarPedidosResultado = 359;
        public static int Visualizar_Aba_Documentos = 633;
        public static class Manutenção_de_Dados // 236
        {
            public static class AtoConcessorio // 237
            {
                public static int Consultar = 238;
                public static int Incluir = 239;
                public static int Alterar = 240;
            }
        }
    }

    public class Requisicao // 11
    {
        public class Manutencao // 272
        {
            public class TipoRequisicao // 273
            {
                public static int Consultar = 273;
                public static int Incluir = 274;
                public static int Alterar = 275;
            }
        }
    }

    public class Qualidade // 285
    {
        public class Manutencao // 286
        {
            public class InstrucaoTecnica // 287
            {
                public static int Consultar = 287;
                public static int Incluir = 288;
                public static int Alterar = 289;
            }
        }

        public static class Acervo // 394
        {
            public static int Consultar = 395;
        }
		
        public static class Sistemas // 550
        {
            public static int Visualizar = 551;
        }
    }

    public static class Compras  // 407
    {
        public static class Pedido_Compras // 408
        {
            public static int Consultar = 409;
            public static int Incluir = 410;
            public static int Alterar = 411;
            public static int IncluirNacional = 412;
            public static int IncluirInternacional = 413;
            public static int ExcluirTarefas = 425;
            public static int AdicionarProdutos = 432;
            public static int Visualizar_Aba_Documentos = 632;															  
        }

        public static class CotacaoCompras // 414
        {
            public static int Consultar = 424;
            public static int Incluir = 422;
            public static int Alterar = 423;
        }
    }

    public static class Impressora // 507
    {
        public static int Consultar = 508;
        public static int Desativar = 509;
        public static int Ativar = 547;
        public static int Incluir = 510;
        public static int Editar = 511;
    }

    public static class FilaImpressao // 541
    {
        public static int Pai = 541;
        public static int Consultar = 542;
        public static int CancelarFilasChk = 543;
        public static int CancelarFila = 544;
        public static int RecolocarFila = 545;
    }

    public static class Fabricacao // 557
    {
        public static int Consultar = 552;
        public static int Gerenciar = 553;
		public static int PDF = 554;							
    }

}
