using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace MigracaoDados
{
    public partial class Form1 : Form
    {
        #region Variáveis
        public const string Diretorio = @"D:\TCC\Estatisticas\";
        public List<PaisesEmMemoria> listaPaisesEmMemoria;
        public SortedList<string, string> paisesPorNome;
        public SortedList<string, string> paisesPorID;
        public List<EstadosEmMemoria> listaEstadosEmMemoria;
        public SortedList<string, string> estadosPorNomePais;
        public List<CidadesEmMemoria> listaCidadesEmMemoria;
        public SortedList<string, string> cidadesPorNomePaisEstado;
        public List<TimesEmMemoria> listaTimesEmMemoria;
        public SortedList<string, string> timesPorNomePaisEstadoCidade;
        public List<CampeonatosEmMemoria> listaCampeonatosEmMemoria;
        public SortedList<string, string> campeonatosPorNome;
        public List<PartidasEmMemoria> listaPartidasEmMemoria;
        public SortedList<string, string> idPaisSedePorEdicao;
        public SortedList<string, string> idPorEdicaoCampeonato;
        public List<RodadaEmMemoria> listaRodadaEmMemoria;
        public SortedList<string, string> idRodadaPorDescricao;
        public List<EstadioEmMemoria> listaEstadiosEmMemoria;
        public SortedList<string, string> estadiosPorNome;
        public List<RodadaPartidaEmMemoria> listaRodadaPartidaEmMemoria;
        public List<EstatisticasEmMemoria> listaEstatisticasEmMemoria;
        public List<PalpitesEmMemoria> listaPalpitesEmMemoria;
        public List<ClassificacaoEmMemoria> listaClassificacaoEmMemoria;
        public List<RegrasEmMemoria> listaRegrasEmMemoria;
        public List<RegraBolaoEmMemoria> listaRegraBolaoEmMemoria;
        public int classAtt = 0;
        public int classMigradas = 0;
        public int estatMigradas = 0;
        public int partidasMigradas = 0;
        public int partidasJaExistem = 0;
        public int partidasAtt = 0;
        public int rodadapartidasMigradas = 0;
        public int rodadasMigradas = 0;
        public int estadiosMigrados = 0;
        public int timesMigrados = 0;
        public int estadosMigrados = 0;
        public int cidadesMigradas = 0;
        public int paisesMigrados = 0;
        public int campeonatosMigrados = 0;
        public enum Tabelas
        {
            Time,
            Partida,
            Estado,
            Pais,
            Cidade,
            Estatisticas,
            Rodada
        }
        public enum Planilhas
        {
            PartidasAllStarNBA,
            PartidasCampBrasileiro,
            PartidasClubesMundo,
            PartidasCopaDoMundo,
            PartidasCopaDoMundo2018,
            PartidasDeSelecoes,
            PartidasNBA,
            PartidasNFL,
            UpdatePartidasCopaDoMundo,
            Estatisticas,
            CrawlerProximasPartidas,
            CrawlerResultadosPartidasPorPais,
            CrawlerResultadosPartidasPorLiga,
            CrawlerProximasPartidasNBA,
            CrawlerResultadosPartidasNBA,
            CrawlerProximasPartidasNBB,
            AutomatizacaoPontos
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
            PreencheComboBoxPlanilhas();
        }

        #region Métodos de criação no BD
        public int CriarRodada(string rodada)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "INSERT INTO Rodada(Descricao) VALUES(@Descricao);SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Descricao", SqlDbType.VarChar, 255).Value = rodada;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarCampeonato(string camp)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "INSERT INTO Campeonato(Nome,idEsporte) VALUES(@Nome,@idEsporte);SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Nome", SqlDbType.VarChar, 255).Value = camp;
                    cmd.Parameters.Add("@idEsporte", SqlDbType.Int).Value = 1;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarEstadio(string nomeEstadio)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "INSERT INTO Estadio(Nome) VALUES(@Nome);SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Nome", SqlDbType.VarChar, 255).Value = nomeEstadio;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarEstado(string siglaEstado, string idPais)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "INSERT INTO Estado(Nome,Ativo,UF,idPais) VALUES(@Nome,@Ativo,@UF,@idPais);SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Nome", SqlDbType.VarChar, 255).Value = RetornarEstadoDosEUA(siglaEstado);
                    cmd.Parameters.Add("@Ativo", SqlDbType.Bit).Value = 1;
                    cmd.Parameters.Add("@UF", SqlDbType.VarChar, 255).Value = siglaEstado;
                    cmd.Parameters.Add("@idPais", SqlDbType.Int).Value = idPais;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarTime(string nomeTime, string pais, string modalidade)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Time(Nome,idPais,Modalidade) VALUES(@Nome,@idPais,@Modalidade);SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Nome", SqlDbType.VarChar, 255).Value = nomeTime;
                    cmd.Parameters.Add("@idPais", SqlDbType.Int).Value = pais;
                    cmd.Parameters.Add("@Modalidade", SqlDbType.Int).Value = modalidade;
                    cmd.CommandType = CommandType.Text;
                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    TimesEmMemoria tm = new TimesEmMemoria
                    {
                        ID = id.ToString(),
                        Nome = RemoveAccents(nomeTime.ToUpper()),
                        Modalidade = modalidade,
                        IdPais = pais
                    };
                    listaTimesEmMemoria.Add(tm);

                    return id;
                }
            }
        }
        public int CriarRodadaPartida(string idPartida, string idRodada)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO rodadapartida(idRodada,idPartida) VALUES(@idRodada,@idPartida);SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idRodada", SqlDbType.Int).Value = Convert.ToInt32(idRodada);
                    cmd.Parameters.Add("@idPartida", SqlDbType.Int).Value = Convert.ToInt32(idPartida);
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public void EditarPosicaoClassificacaoNoBanco(InfoClassificacao dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();

                string sql = @"Update Classificacao set Posicao = @Posicao, Variacao = @Variacao, PosicaoAnterior = @PosicaoAnterior where id = @id;";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@PosicaoAnterior", SqlDbType.Int).Value = dados.PosicaoAnterior;
                    cmd.Parameters.Add("@Posicao", SqlDbType.Int).Value = dados.Posicao;
                    cmd.Parameters.Add("@Variacao", SqlDbType.Int).Value = dados.Variacao;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = dados.ID;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateContabilizadoPalpite(int idPalpite)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();

                string sql = @"Update Palpites set Contabilizado = 1 where id = @id;";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPalpite;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void EditarClassificacaoNoBanco(InfoClassificacao dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();

                string sql = @"Update Classificacao set AcertouVencedor = @AcertouVencedor, PlacarCheio = @PlacarCheio, PlacarPerdedor = @PlacarPerdedor," +
                              "PlacarVencedor = @PlacarVencedor, PosicaoAnterior = @PosicaoAnterior, total = @total where id = @id;";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@AcertouVencedor", SqlDbType.Int).Value = dados.AcertouVencedor;
                    cmd.Parameters.Add("@PlacarCheio", SqlDbType.Int).Value = dados.PlacarCheio;
                    cmd.Parameters.Add("@PlacarPerdedor", SqlDbType.Int).Value = dados.PlacarPerdedor;
                    cmd.Parameters.Add("@PlacarVencedor", SqlDbType.Int).Value = dados.PlacarVencedor;
                    cmd.Parameters.Add("@posicaoAnterior", SqlDbType.Int).Value = dados.PosicaoAnterior;
                    cmd.Parameters.Add("@total", SqlDbType.Float).Value = dados.Total;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = dados.ID;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int CriarClassificacaoNoBanco(InfoClassificacao dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Classificacao(idBolao,idUsuario,total,PlacarCheio,PlacarVencedor,PlacarPerdedor,Variacao,AcertouVencedor,posicao,posicaoAnterior) " +
                    "VALUES(@idBolao,@idUsuario,@total,@PlacarCheio,@PlacarVencedor,@PlacarPerdedor,@Variacao,@AcertouVencedor,@posicao,@posicaoAnterior);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idBolao", SqlDbType.Int).Value = dados.IdBolao;
                    cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = dados.IdUsuario;
                    cmd.Parameters.Add("@total", SqlDbType.Float).Value = dados.Total;
                    cmd.Parameters.Add("@PlacarCheio", SqlDbType.Int).Value = dados.PlacarCheio;
                    cmd.Parameters.Add("@PlacarVencedor", SqlDbType.Int).Value = dados.PlacarVencedor;
                    cmd.Parameters.Add("@PlacarPerdedor", SqlDbType.Int).Value = dados.PlacarPerdedor;
                    cmd.Parameters.Add("@Variacao", SqlDbType.Int).Value = dados.Variacao;
                    cmd.Parameters.Add("@AcertouVencedor", SqlDbType.Int).Value = dados.AcertouVencedor;
                    cmd.Parameters.Add("@posicao", SqlDbType.Int).Value = dados.Posicao;
                    cmd.Parameters.Add("@posicaoAnterior", SqlDbType.Int).Value = dados.PosicaoAnterior;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarEstatisticasNoBanco(InfoEstatisticas dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Estatisticas(idTimeA,idTimeB,vitoriasTimeA,vitoriasTimeB,empates,total) " +
                    "VALUES(@idTimeA,@idTimeB,@vitoriasTimeA,@vitoriasTimeB,@empates,@total);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeA", SqlDbType.Int).Value = dados.IdTimeCasa;
                    cmd.Parameters.Add("@idTimeB", SqlDbType.Int).Value = dados.IdTimeFora;
                    cmd.Parameters.Add("@vitoriasTimeA", SqlDbType.Int).Value = dados.Vitorias;
                    cmd.Parameters.Add("@vitoriasTimeB", SqlDbType.Int).Value = dados.Derrotas;
                    cmd.Parameters.Add("@empates", SqlDbType.Int).Value = dados.Empates;
                    cmd.Parameters.Add("@total", SqlDbType.Int).Value = dados.Total;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPartida(Dados dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Partida(idTimeCasa,idTimeFora,idCampeonato,data,placarTimeCasa,placarTimeFora,idestadio) " +
                    "VALUES(@idTimeCasa,@idTimeFora,@idCampeonato,@data,@placarTimeCasa,@placarTimeFora,@idestadio);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeCasa", SqlDbType.Int).Value = Convert.ToInt32(dados.TimeCasa);
                    cmd.Parameters.Add("@idTimeFora", SqlDbType.Int).Value = Convert.ToInt32(dados.TimeFora);
                    cmd.Parameters.Add("@idCampeonato", SqlDbType.Int).Value = Convert.ToInt32(dados.Campeonato);
                    cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = dados.Data;
                    cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = dados.PlacarTimeCasa;
                    cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = dados.PlacarTimeFora;
                    cmd.Parameters.Add("@idestadio", SqlDbType.Int).Value = Convert.ToInt32(dados.Arena);
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPartida(Jogos jogos)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Partida(idTimeCasa,idTimeFora,idCampeonato,data,placarTimeCasa,placarTimeFora,idConversao) " +
                    "VALUES(@idTimeCasa,@idTimeFora,@idCampeonato,@data,@placarTimeCasa,@placarTimeFora,@idConversao);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeCasa", SqlDbType.Int).Value = Convert.ToInt32(jogos.TimeCasa);
                    cmd.Parameters.Add("@idTimeFora", SqlDbType.Int).Value = Convert.ToInt32(jogos.TimeFora);
                    cmd.Parameters.Add("@idCampeonato", SqlDbType.Int).Value = Convert.ToInt32(jogos.Campeonato);
                    cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = jogos.Data;
                    cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = jogos.PlacarCasa;
                    cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = jogos.PlacarFora;
                    cmd.Parameters.Add("@idConversao", SqlDbType.VarChar).Value = jogos.IdPartida.ToString();
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPartidaNBA(Dados dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Partida(idTimeCasa,idTimeFora,idCampeonato,data,placarTimeCasa,placarTimeFora) " +
                    "VALUES(@idTimeCasa,@idTimeFora,@idCampeonato,@data,@placarTimeCasa,@placarTimeFora);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeCasa", SqlDbType.Int).Value = Convert.ToInt32(dados.TimeCasa);
                    cmd.Parameters.Add("@idTimeFora", SqlDbType.Int).Value = Convert.ToInt32(dados.TimeFora);
                    cmd.Parameters.Add("@idCampeonato", SqlDbType.Int).Value = Convert.ToInt32(dados.Campeonato);
                    cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = dados.Data;
                    cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = dados.PlacarTimeCasa;
                    cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = dados.PlacarTimeFora;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPartidaNBAAllStar(Dados dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Partida(idTimeCasa,idTimeFora,idCampeonato,data,placarTimeCasa,placarTimeFora,idEstadio) " +
                    "VALUES(@idTimeCasa,@idTimeFora,@idCampeonato,@data,@placarTimeCasa,@placarTimeFora,@idEstadio);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeCasa", SqlDbType.Int).Value = Convert.ToInt32(dados.TimeCasa);
                    cmd.Parameters.Add("@idTimeFora", SqlDbType.Int).Value = Convert.ToInt32(dados.TimeFora);
                    cmd.Parameters.Add("@idCampeonato", SqlDbType.Int).Value = Convert.ToInt32(dados.Campeonato);
                    cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = dados.Data;
                    cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = dados.PlacarTimeCasa;
                    cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = dados.PlacarTimeFora;
                    cmd.Parameters.Add("@idEstadio", SqlDbType.Int).Value = dados.Arena;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPartidaCopaDoMundo(Dados dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Partida(idTimeCasa,idTimeFora,idCampeonato,data,placarTimeCasa,placarTimeFora) " +
                    "VALUES(@idTimeCasa,@idTimeFora,@idCampeonato,@data,@placarTimeCasa,@placarTimeFora);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeCasa", SqlDbType.Int).Value = dados.TimeCasa;
                    cmd.Parameters.Add("@idTimeFora", SqlDbType.Int).Value = dados.TimeFora;
                    cmd.Parameters.Add("@idCampeonato", SqlDbType.Int).Value = dados.Campeonato;
                    if (cbPlanilha.SelectedItem.ToString() == "PartidasCopaDoMundo2018")
                        cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = dados.Data;
                    else
                        cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = MontarData(dados.Data);
                    cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = dados.PlacarTimeCasa;
                    cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = dados.PlacarTimeFora;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPartidaDeSelecoes(Dados dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Partida(idTimeCasa,idTimeFora,idCampeonato,data,placarTimeCasa,placarTimeFora) " +
                    "VALUES(@idTimeCasa,@idTimeFora,@idCampeonato,@data,@placarTimeCasa,@placarTimeFora);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeCasa", SqlDbType.Int).Value = dados.TimeCasa;
                    cmd.Parameters.Add("@idTimeFora", SqlDbType.Int).Value = dados.TimeFora;
                    cmd.Parameters.Add("@idCampeonato", SqlDbType.Int).Value = dados.Campeonato;
                    cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = dados.Data;
                    cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = dados.PlacarTimeCasa;
                    cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = dados.PlacarTimeFora;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPartidaDeClubesMundo(Dados dados)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = @"INSERT INTO Partida(idTimeCasa,idTimeFora,idCampeonato,data,placarTimeCasa,placarTimeFora) " +
                    "VALUES(@idTimeCasa,@idTimeFora,@idCampeonato,@data,@placarTimeCasa,@placarTimeFora);" +
                    "SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@idTimeCasa", SqlDbType.Int).Value = dados.TimeCasa;
                    cmd.Parameters.Add("@idTimeFora", SqlDbType.Int).Value = dados.TimeFora;
                    cmd.Parameters.Add("@idCampeonato", SqlDbType.Int).Value = dados.Campeonato;
                    cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = dados.Data;
                    cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = dados.PlacarTimeCasa;
                    cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = dados.PlacarTimeFora;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public void CriarRodadaPartida(string rodada, int idPartida)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "INSERT INTO rodadaPartida(idRodada,idPartida) VALUES(@idRodada,@idPartida)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    if (idRodadaPorDescricao.ContainsKey(RemoveAccents(rodada).ToUpper()))
                        cmd.Parameters.Add("@idRodada", SqlDbType.Int).Value = Convert.ToInt32(idRodadaPorDescricao[RemoveAccents(rodada).ToUpper()]);
                    cmd.Parameters.Add("@idPartida", SqlDbType.Int).Value = idPartida;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int CriarCidades(string nomeCidade, int idEstado, int idPais)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "";
                if (idEstado != -1)
                {
                    sql = "INSERT INTO cidade(Nome,Ativo,idPais,idEstado) VALUES(@Nome,@Ativo,@idPais,@idEstado);SELECT SCOPE_IDENTITY()";
                }
                else
                {
                    sql = "INSERT INTO cidade(Nome,Ativo,idPais) VALUES(@Nome,@Ativo,@idPais);SELECT SCOPE_IDENTITY()";
                }
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Nome", SqlDbType.VarChar).Value = nomeCidade;
                    cmd.Parameters.Add("@Ativo", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@idPais", SqlDbType.Int).Value = idPais;
                    if (idEstado != -1)
                        cmd.Parameters.Add("@idEstado", SqlDbType.Int).Value = idEstado;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CriarPaises(string nomePais)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "INSERT INTO Pais(Nome,Ativo) VALUES(@Nome,@Ativo);SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Nome", SqlDbType.VarChar).Value = nomePais;
                    cmd.Parameters.Add("@Ativo", SqlDbType.Int).Value = 1;
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public void CriarSelecoes(string selecao, int idPais)
        {
            using (SqlConnection connection = new SqlConnection(tbConexao.Text))
            {
                connection.Open();
                string sql = "INSERT INTO Time(Nome,idPais) VALUES(@Nome,@idPais)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@Nome", SqlDbType.VarChar).Value = selecao;
                    cmd.Parameters.Add("@idPais", SqlDbType.Int).Value = idPais;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateDataCopaMundo()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv")
                    .Select(a => a.Split(';'))
                    .Select(c => new Dados()
                    {
                        Edicao = c[0],
                        Data = c[1]
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.Edicao == "ID")
                {
                    continue;
                }
                using (SqlConnection connection = new SqlConnection(tbConexao.Text))
                {
                    connection.Open();
                    string sql = "update Partida set Data = @data where id = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = info.Edicao;
                        cmd.Parameters.Add("@data", SqlDbType.DateTime).Value = MontarData(info.Data);
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion

        #region Métodos Auxiliares
        public void PreencheComboBoxPlanilhas()
        {
            foreach (Planilhas plan in Enum.GetValues(typeof(Planilhas)))
            {
                cbPlanilha.Items.Add(plan);
            }
        }
        private string RemoveAccents(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }
        private string MontarData(string data)
        {
            string[] teste = data.Split(' ');
            string[] teste2 = teste[4].Split(':');
            DateTime dt = new DateTime(Convert.ToInt32(teste[2]), RetornaMes(teste[1].ToUpper()), Convert.ToInt32(teste[0]), Convert.ToInt32(teste2[0]), Convert.ToInt32(teste2[1]), 0);
            return dt.ToString();
        }
        private int RetornaMes(string mes)
        {
            switch (mes.ToUpper())
            {
                case "JAN":
                    return 1;
                case "FEV":
                case "FEB":
                    return 2;
                case "MAR":
                    return 3;
                case "ABR":
                case "APR":
                    return 4;
                case "MAY":
                    return 5;
                case "JUN":
                case "JUNE":
                    return 6;
                case "JUL":
                case "JULY":
                    return 7;
                case "AGO":
                    return 8;
                case "SET":
                    return 9;
                case "OUT":
                case "OCT":
                    return 10;
                case "NOV":
                    return 11;
                case "DEZ":
                case "DEC":
                    return 12;
                default:
                    return -1;
            }
        }
        private void AtualizarResultado()
        {
            tbResultado.Text = $"Partidas migradas: {partidasMigradas}\r\n" +
                               $"Partidas atualizadas: {partidasAtt}\r\n" +
                               $"Partidas já existem: {partidasJaExistem}\r\n" +
                               $"Estádios migrados: {estadiosMigrados}\r\n" +
                               $"Times migrados: {timesMigrados}\r\n" +
                               $"Rodadas migradas: {rodadasMigradas}\r\n" +
                               $"Rodada/Partida migrados: {rodadapartidasMigradas}\r\n" +
                               $"Classificações migradas: {classMigradas}\r\n" +
                               $"Classificações atualizadas: {classAtt}\r\n" +
                               $"Estatísticas: {estatMigradas}";
        }
        private string RetornarEstadoDosEUA(string siglaEstado)
        {
            switch (siglaEstado)
            {
                case "AK":
                    return "Alaska";
                case "AL":
                    return "Alabama";
                case "AR":
                    return "Arkansas";
                case "AZ":
                    return "Arizona";
                case "CA":
                    return "California";
                case "CO":
                    return "Colorado";
                case "CT":
                    return "Connecticut";
                case "DC":
                    return "District of Columbia";
                case "DE":
                    return "Delaware";
                case "FL":
                    return "Florida";
                case "GA":
                    return "Georgia";
                case "HI":
                    return "Hawaii";
                case "IA":
                    return "Iowa";
                case "ID":
                    return "Idaho";
                case "IL":
                    return "Illinois";
                case "IN":
                    return "Indiana";
                case "KS":
                    return "Kansas";
                case "KY":
                    return "Kentucky";
                case "LA":
                    return "Louisiana";
                case "MD":
                    return "Maryland";
                case "ME":
                    return "Maine";
                case "MI":
                    return "Michigan";
                case "MN":
                    return "Minnesota";
                case "MO":
                    return "Missouri";
                case "MS":
                    return "Mississippi";
                case "MT":
                    return "Montana";
                case "NC":
                    return "North Carolina";
                case "ND":
                    return "North Dakota";
                case "NE":
                    return "Nebraska";
                case "NH":
                    return "New Hampshire";
                case "NJ":
                    return "New Jersey";
                case "NM":
                    return "New Mexico";
                case "NV":
                    return "Nevada";
                case "NY":
                    return "New York";
                case "OH":
                    return "Ohio";
                case "OK":
                    return "Oklahoma";
                case "OR":
                    return "Oregon";
                case "PA":
                    return "Pennsylvania";
                case "RI":
                    return "Rhode Island";
                case "SC":
                    return "South Carolina";
                case "SD":
                    return "South Dakota";
                case "TN":
                    return "Tennessee";
                case "TX":
                    return "Texas";
                case "UT":
                    return "Utah";
                case "VT":
                    return "Vermont";
                case "VA":
                    return "Virginia";
                case "WA":
                    return "Washington";
                case "WI":
                    return "Wisconsin";
                case "WV":
                    return "West Virginia";
                case "WY":
                    return "Wyoming";
                case "MA":
                    return "Massachusetts";
                default:
                    return "";
            }
        }
        #endregion

        #region Métodos de busca no BD - Destino
        public DataTable ExecutaBuscaSql(string Select)
        {
            DataTable rdrDt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(tbConexao.Text))
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = Select;
                    sqlCommand.CommandTimeout = 0;
                    sqlCommand.ExecuteNonQuery();

                    SqlDataAdapter Adp = new SqlDataAdapter(sqlCommand);
                    Adp.Fill(rdrDt);
                    sqlConnection.Close();
                }
            }
            return rdrDt;
        }
        #endregion

        #region Métodos verificam se o registro existe
        public bool VerificaSeEstatisticaExiste(string timeCasa, string timeFora)
        {
            return listaEstatisticasEmMemoria.Exists(p => (p.IdTimeA == timeCasa && p.IdTimeB == timeFora) || (p.IdTimeB == timeCasa && p.IdTimeA == timeFora));
        }
        public bool VerificaSeClassificacaoExiste(string idUsuario, string idBolao)
        {
            return listaClassificacaoEmMemoria.Exists(p => p.IdUsuario == idUsuario && p.IdBolao == idBolao);
        }
        public bool VerificaSePartidaExiste(string timeCasa, string timeFora, string data)
        {
            if (listaPartidasEmMemoria.Exists(p => p.IdTimeCasa == timeCasa && p.IdTimeFora == timeFora && Convert.ToDateTime(p.Data).Date == Convert.ToDateTime(data).Date))
            {
                return true;
            }
            else
            {
                return listaPartidasEmMemoria.Exists(p => p.IdTimeCasa == timeFora && p.IdTimeFora == timeCasa && Convert.ToDateTime(p.Data).Date == Convert.ToDateTime(data).Date);
            }
        }
        public bool VerificaSePartidaExisteIdConversao(string idConversao)
        {
            return listaPartidasEmMemoria.Exists(p => p.IdConversao == idConversao);
        }
        public bool VerificaSeRodadaPartidaExiste(string idPartida, string rodada)
        {
            return listaRodadaPartidaEmMemoria.Exists(p => p.IdPartida == idPartida && p.IdRodada == rodada);
        }
        #endregion

        #region Métodos retornam informações em memória
        public string RetornaIDTime(string nomeTime, string modalidade)
        {
            if (nomeTime == "França")
            {
                return "195";
            }

            int qtdTimesPorNome = listaTimesEmMemoria.Count(p => p.Nome == RemoveAccents(nomeTime).ToUpper() && p.Modalidade == modalidade);
            if (qtdTimesPorNome == 1)
            {
                return listaTimesEmMemoria.Find(p => p.Nome == RemoveAccents(nomeTime).ToUpper() && p.Modalidade == modalidade).ID;
            }
            else if (qtdTimesPorNome > 1)
            {
                return "";
            }
            else
            {
                return "";
            }
        }
        private string RetornaIDCampeonato(string camp)
        {
            if (campeonatosPorNome.ContainsKey(RemoveAccents(camp).ToUpper()))
            {
                return campeonatosPorNome[RemoveAccents(camp).ToUpper()];
            }
            else
            {
                return "";
            }
        }
        private string RetornaIDPais(string pais)
        {
            if (paisesPorNome.ContainsKey(RemoveAccents(pais).ToUpper()))
            {
                return paisesPorNome[RemoveAccents(pais).ToUpper()];
            }
            else
            {
                return "";
            }
        }
        private string RetornaIDEstadio(string arena)
        {
            if (estadiosPorNome.ContainsKey(RemoveAccents(arena).ToUpper()))
            {
                return estadiosPorNome[RemoveAccents(arena).ToUpper()];
            }
            else
            {
                return "";
            }
        }
        private string RetornaIDEstado(string siglaEstado)
        {
            if (estadosPorNomePais.ContainsKey(RemoveAccents(RetornarEstadoDosEUA(siglaEstado)).ToUpper() + ";ESTADOS UNIDOS"))
            {
                return estadosPorNomePais[RemoveAccents(RetornarEstadoDosEUA(siglaEstado)).ToUpper() + ";ESTADOS UNIDOS"];
            }
            else
            {
                return "";
            }
        }
        private string RetornaIDCidade(string nomeCidade, string siglaEstado = "", string pais = "ESTADOS UNIDOS")
        {
            if (cidadesPorNomePaisEstado.ContainsKey(RemoveAccents(nomeCidade).ToUpper() + ";" + RemoveAccents(pais).ToUpper() + ";" + RemoveAccents(RetornarEstadoDosEUA(siglaEstado)).ToUpper()))
            {
                return cidadesPorNomePaisEstado[RemoveAccents(nomeCidade).ToUpper() + ";" + RemoveAccents(pais).ToUpper() + ";" + RemoveAccents(RetornarEstadoDosEUA(siglaEstado)).ToUpper()];
            }
            else
            {
                return "";
            }
        }
        private string RetornaIDRodada(string rodada)
        {
            if (idRodadaPorDescricao.ContainsKey(RemoveAccents(rodada).ToUpper()))
            {
                return idRodadaPorDescricao[RemoveAccents(rodada).ToUpper()];
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Armazena em memória as informações de Destino
        public void BuscaRegraBolaoEmMemoria()
        {
            listaRegraBolaoEmMemoria = new List<RegraBolaoEmMemoria>();
            string auxSql = @"Select * from regrabolao";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                RegraBolaoEmMemoria regraBolaoAtual = new RegraBolaoEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    IdBolao = dtrow["IdBolao"].ToString(),
                    IdRegra = dtrow["IdRegra"].ToString()
                };
                listaRegraBolaoEmMemoria.Add(regraBolaoAtual);
            }
        }
        public void BuscaRegrasEmMemoria()
        {
            listaRegrasEmMemoria = new List<RegrasEmMemoria>();
            string auxSql = @"Select * from Regras";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                RegrasEmMemoria regraAtual = new RegrasEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    Pontuacao1 = dtrow["Pontuacao1"].ToString(),
                    Pontuacao2 = dtrow["Pontuacao2"].ToString(),
                    Pontuacao3 = dtrow["Pontuacao3"].ToString()
                };
                listaRegrasEmMemoria.Add(regraAtual);
            }
        }
        public void BuscaClassificacaoEmMemoria()
        {
            listaClassificacaoEmMemoria = new List<ClassificacaoEmMemoria>();
            string auxSql = @"Select * from Classificacao";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                ClassificacaoEmMemoria classificacaoAtual = new ClassificacaoEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    IdUsuario = dtrow["idUsuario"].ToString(),
                    IdBolao = dtrow["idBolao"].ToString(),
                    Total = dtrow["total"].ToString(),
                    PlacarCheio = dtrow["PlacarCheio"].ToString(),
                    PlacarVencedor = dtrow["PlacarVencedor"].ToString(),
                    PlacarPerdedor = dtrow["PlacarPerdedor"].ToString(),
                    AcertouVencedor = dtrow["AcertouVencedor"].ToString(),
                    Variacao = dtrow["Variacao"].ToString(),
                    Posicao = dtrow["Posicao"].ToString(),
                    PosicaoAnterior = dtrow["PosicaoAnterior"].ToString()
                };
                listaClassificacaoEmMemoria.Add(classificacaoAtual);
            }
        }
        public void BuscaPalpitesEmMemoria()
        {
            listaPalpitesEmMemoria = new List<PalpitesEmMemoria>();
            string auxSql = @"Select * from Palpites";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                PalpitesEmMemoria palpiteAtual = new PalpitesEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    IdBolao = dtrow["idBolao"].ToString(),
                    IdPartida = dtrow["idPartida"].ToString(),
                    IdTimeCasa = dtrow["idTimeCasa"].ToString(),
                    IdTimeFora = dtrow["idTimeFora"].ToString(),
                    IdUsuario = dtrow["idUsuario"].ToString(),
                    PalpiteTimeCasa = dtrow["palpiteTimeCasa"].ToString(),
                    PalpiteTimeFora = dtrow["palpiteTimeFora"].ToString(),
                    Contabilizado = dtrow["contabilizado"].ToString()
                };
                listaPalpitesEmMemoria.Add(palpiteAtual);
            }
        }
        public void BuscaEstatisticasEmMemoria()
        {
            listaEstatisticasEmMemoria = new List<EstatisticasEmMemoria>();
            string auxSql = @"Select e.ID,e.idTimeA,e.idTimeB,e.vitoriasTimeA,e.vitoriasTimeB,e.empates,t.Nome as TimeA, t1.Nome as TimeB from Estatisticas e left join Time t on(t.id = e.idTimeA) left join Time t1 on(t1.id = e.idTimeB)";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                EstatisticasEmMemoria estatisticaAtual = new EstatisticasEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    IdTimeA = dtrow["idTimeA"].ToString(),
                    IdTimeB = dtrow["idTimeB"].ToString(),
                    NomeTimeA = RemoveAccents(dtrow["TimeA"].ToString().Trim().ToUpper()),
                    NomeTimeB = RemoveAccents(dtrow["TimeB"].ToString().Trim().ToUpper()),
                    VitoriasTimeA = dtrow["vitoriasTimeA"].ToString(),
                    VitoriasTimeB = dtrow["vitoriasTimeB"].ToString(),
                    Empates = dtrow["empates"].ToString()
                };
                listaEstatisticasEmMemoria.Add(estatisticaAtual);
            }
        }
        public void BuscaTimesEmMemoria()
        {
            timesPorNomePaisEstadoCidade = new SortedList<string, string>();
            listaTimesEmMemoria = new List<TimesEmMemoria>();
            string auxSql = @"Select t.ID as ID,t.Nome as Nome,t.idCidade as idCidade,t.idEstado as idEstado,t.idPais as idPais,t.Modalidade as modalidade,p.Nome as NomePais, e.Nome as NomeEstado, c.Nome as NomeCidade from Time t left join Pais p on(p.id = t.idPais) left join Estado e on(e.id = t.idEstado) left join Cidade c on(c.id = t.idCidade) order by Nome";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                TimesEmMemoria timeAtual = new TimesEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    Nome = RemoveAccents(dtrow["Nome"].ToString().Trim().ToUpper()),
                    IdPais = dtrow["idPais"].ToString(),
                    IdEstado = dtrow["idEstado"].ToString(),
                    IdCidade = dtrow["idCidade"].ToString(),
                    NomePais = RemoveAccents(dtrow["NomePais"].ToString().Trim().ToUpper()),
                    NomeEstado = RemoveAccents(dtrow["NomeEstado"].ToString().Trim().ToUpper()),
                    NomeCidade = RemoveAccents(dtrow["NomeCidade"].ToString().Trim().ToUpper()),
                    Modalidade = dtrow["modalidade"].ToString()
                };

                if (!timesPorNomePaisEstadoCidade.ContainsKey($"{timeAtual.Nome};{timeAtual.NomePais};{timeAtual.NomeEstado};{timeAtual.NomeCidade}"))
                    timesPorNomePaisEstadoCidade.Add($"{timeAtual.Nome};{timeAtual.NomePais};{timeAtual.NomeEstado};{timeAtual.NomeCidade}", timeAtual.ID);
                listaTimesEmMemoria.Add(timeAtual);
            }
        }
        public void BuscaEstadiosEmMemoria()
        {
            estadiosPorNome = new SortedList<string, string>();
            listaEstadiosEmMemoria = new List<EstadioEmMemoria>();
            string auxSql = "Select Nome,ID from Estadio order by Nome";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                EstadioEmMemoria estadioAtual = new EstadioEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    Nome = RemoveAccents(dtrow["Nome"].ToString().Trim().ToUpper())
                };

                if (!estadiosPorNome.ContainsKey(estadioAtual.Nome))
                    estadiosPorNome.Add(estadioAtual.Nome, estadioAtual.ID);
                listaEstadiosEmMemoria.Add(estadioAtual);
            }
        }
        public void BuscaPaisesEmMemoria()
        {
            paisesPorNome = new SortedList<string, string>();
            paisesPorID = new SortedList<string, string>();
            listaPaisesEmMemoria = new List<PaisesEmMemoria>();
            string auxSql = "Select Nome,ID from Pais order by Nome";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                PaisesEmMemoria paisAtual = new PaisesEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    Nome = RemoveAccents(dtrow["Nome"].ToString().Trim().ToUpper())
                };

                if (!paisesPorNome.ContainsKey(paisAtual.Nome))
                    paisesPorNome.Add(paisAtual.Nome, paisAtual.ID);
                if (!paisesPorID.ContainsKey(paisAtual.ID))
                    paisesPorID.Add(paisAtual.ID, paisAtual.Nome);
                listaPaisesEmMemoria.Add(paisAtual);
            }
        }
        public void BuscaEstadosEmMemoria()
        {
            estadosPorNomePais = new SortedList<string, string>();
            listaEstadosEmMemoria = new List<EstadosEmMemoria>();
            string auxSql = "Select e.ID as ID,e.Nome as Nome,e.idPais as idPais,p.Nome as NomePais from Estado e left join Pais p on(p.id = e.idPais) order by Nome";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                EstadosEmMemoria estadoAtual = new EstadosEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    Nome = RemoveAccents(dtrow["Nome"].ToString().Trim().ToUpper()),
                    IdPais = dtrow["idPais"].ToString(),
                    NomePais = RemoveAccents(dtrow["NomePais"].ToString().Trim().ToUpper())
                };

                if (!estadosPorNomePais.ContainsKey($"{estadoAtual.Nome};{estadoAtual.NomePais}"))
                    estadosPorNomePais.Add($"{estadoAtual.Nome};{estadoAtual.NomePais}", estadoAtual.ID);
                listaEstadosEmMemoria.Add(estadoAtual);
            }
        }
        public void BuscaCidadesEmMemoria()
        {
            cidadesPorNomePaisEstado = new SortedList<string, string>();
            listaCidadesEmMemoria = new List<CidadesEmMemoria>();
            string auxSql = @"Select c.ID as ID,c.Nome as Nome,c.idEstado as idEstado,c.idPais as idPais,p.Nome as NomePais, e.Nome as NomeEstado from Cidade c left join Pais p on(p.id = c.idPais) left join Estado e on(e.id = c.idEstado) order by Nome";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                CidadesEmMemoria cidadeAtual = new CidadesEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    Nome = RemoveAccents(dtrow["Nome"].ToString().Trim().ToUpper()),
                    IdPais = dtrow["idPais"].ToString(),
                    IdEstado = dtrow["idEstado"].ToString(),
                    NomePais = RemoveAccents(dtrow["NomePais"].ToString().Trim().ToUpper()),
                    NomeEstado = RemoveAccents(dtrow["NomeEstado"].ToString().Trim().ToUpper())
                };

                if (!cidadesPorNomePaisEstado.ContainsKey($"{cidadeAtual.Nome};{cidadeAtual.NomePais};{cidadeAtual.NomeEstado}"))
                    cidadesPorNomePaisEstado.Add($"{cidadeAtual.Nome};{cidadeAtual.NomePais};{cidadeAtual.NomeEstado}", cidadeAtual.ID);
                listaCidadesEmMemoria.Add(cidadeAtual);
            }
        }
        public void BuscaCampeonatosEmMemoria()
        {
            campeonatosPorNome = new SortedList<string, string>();
            listaCampeonatosEmMemoria = new List<CampeonatosEmMemoria>();
            string auxSql = "Select Nome,ID from Campeonato order by Nome";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                CampeonatosEmMemoria campeonatoAtual = new CampeonatosEmMemoria
                {
                    ID = dtrow["ID"].ToString().Trim(),
                    Nome = RemoveAccents(dtrow["Nome"].ToString().Trim().ToUpper())
                };

                if (!campeonatosPorNome.ContainsKey(campeonatoAtual.Nome))
                    campeonatosPorNome.Add(campeonatoAtual.Nome, campeonatoAtual.ID);
                listaCampeonatosEmMemoria.Add(campeonatoAtual);
            }
        }
        public void BuscaPartidasEmMemoria()
        {
            listaPartidasEmMemoria = new List<PartidasEmMemoria>();
            string auxSql = "Select * from Partida order by Data";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                PartidasEmMemoria partidaAtual = new PartidasEmMemoria
                {
                    ID = dtrow["ID"].ToString(),
                    IdTimeCasa = dtrow["IdTimeCasa"].ToString(),
                    IdTimeFora = dtrow["IdTimeFora"].ToString(),
                    IdCampeonato = dtrow["IdCampeonato"].ToString(),
                    Data = dtrow["Data"].ToString(),
                    PlacarTimeCasa = dtrow["PlacarTimeCasa"].ToString(),
                    PlacarTimeFora = dtrow["PlacarTimeFora"].ToString(),
                    IdEstadio = dtrow["IdEstadio"].ToString(),
                    IdConversao = dtrow["IdConversao"].ToString()
                };
                listaPartidasEmMemoria.Add(partidaAtual);
            }
        }
        public void BuscaRodadaEmMemoria()
        {
            listaRodadaEmMemoria = new List<RodadaEmMemoria>();
            idRodadaPorDescricao = new SortedList<string, string>();
            string auxSql = "Select * from Rodada order by Descricao";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                RodadaEmMemoria rodadaAtual = new RodadaEmMemoria
                {
                    ID = dtrow["ID"].ToString(),
                    Descricao = RemoveAccents(dtrow["Descricao"].ToString()).ToUpper()
                };
                if (!idRodadaPorDescricao.ContainsKey(rodadaAtual.Descricao))
                    idRodadaPorDescricao.Add(rodadaAtual.Descricao, rodadaAtual.ID);
                listaRodadaEmMemoria.Add(rodadaAtual);
            }
        }
        public void BuscaRodadaPartidaEmMemoria()
        {
            listaRodadaPartidaEmMemoria = new List<RodadaPartidaEmMemoria>();
            string auxSql = "Select * from rodadapartida";
            DataTable dtTable = ExecutaBuscaSql(auxSql);
            foreach (DataRow dtrow in dtTable.Rows)
            {
                RodadaPartidaEmMemoria rodadaPartidaAtual = new RodadaPartidaEmMemoria
                {
                    ID = dtrow["ID"].ToString(),
                    IdPartida = dtrow["IdPartida"].ToString(),
                    IdRodada = dtrow["IdRodada"].ToString()
                };
                listaRodadaPartidaEmMemoria.Add(rodadaPartidaAtual);
            }
        }
        #endregion

        #region Ações Botões
        private void btIniciarMigracao_Click(object sender, EventArgs e)
        {
            BuscaRodadaPartidaEmMemoria();
            BuscaEstadiosEmMemoria();
            BuscaCampeonatosEmMemoria();
            BuscaPaisesEmMemoria();
            BuscaEstadosEmMemoria();
            BuscaCidadesEmMemoria();
            BuscaPartidasEmMemoria();
            BuscaRodadaEmMemoria();
            BuscaTimesEmMemoria();
            BuscaEstatisticasEmMemoria();
            BuscaPalpitesEmMemoria();
            BuscaClassificacaoEmMemoria();
            BuscaRegrasEmMemoria();
            BuscaRegraBolaoEmMemoria();

            switch ((Planilhas)cbPlanilha.SelectedItem)
            {
                case Planilhas.PartidasCampBrasileiro:
                    CriarPartidasCampeonatoBrasileiro();
                    break;
                case Planilhas.PartidasClubesMundo:
                    CriarPartidasDeClubesMundo();
                    break;
                case Planilhas.PartidasCopaDoMundo:
                    CriarPartidasCopaDoMundo();
                    break;
                case Planilhas.PartidasCopaDoMundo2018:
                    CriarPartidasCopaDoMundo2018();
                    break;
                case Planilhas.PartidasDeSelecoes:
                    CriarPartidasSelecoes();
                    break;
                case Planilhas.PartidasNBA:
                    CriarPartidasNBA();
                    break;
                case Planilhas.PartidasNFL:
                    CriarPartidasNFL();
                    break;
                case Planilhas.PartidasAllStarNBA:
                    CriarPartidasAllStarNBA();
                    break;
                case Planilhas.UpdatePartidasCopaDoMundo:
                    UpdateDataCopaMundo();
                    break;
                case Planilhas.Estatisticas:
                    CriarEstatisticas();
                    break;
                case Planilhas.CrawlerProximasPartidas:
                    CrawlerBuscarProximasPartidas();
                    break;
                case Planilhas.CrawlerResultadosPartidasPorPais:
                    CrawlerResultadosPartidasPorPais();
                    break;
                case Planilhas.CrawlerResultadosPartidasPorLiga:
                    CrawlerResultadosPartidasPorLiga();
                    break;
                case Planilhas.CrawlerProximasPartidasNBA:
                    CrawlerProximasPartidasNBA();
                    break;
                case Planilhas.CrawlerResultadosPartidasNBA:
                    CrawlerResultadosPartidasNBA();
                    break;
                case Planilhas.CrawlerProximasPartidasNBB:
                    CrawlerProximasPartidasNBB();
                    break;
                case Planilhas.AutomatizacaoPontos:
                    AutomatizacaoPontos();
                    break;
                default:
                    tbResultado.Text = "Nenhuma planilha foi selecionada para migração. Favor selecionar uma planilhas";
                    break;
            }
        }

        public void AutomatizacaoPontos()
        {
            List<InfoClassificacao> listainfoClassificacao = new List<InfoClassificacao>();

            foreach (var item in listaPalpitesEmMemoria.Where(p => p.Contabilizado == "False").OrderByDescending(o => o.IdBolao))
            {
                PartidasEmMemoria partida = listaPartidasEmMemoria.Where(d => d.ID == item.IdPartida).FirstOrDefault();
                RegraBolaoEmMemoria regraBolao = listaRegraBolaoEmMemoria.Where(rb => rb.IdBolao == item.IdBolao).FirstOrDefault();
                RegrasEmMemoria regra = listaRegrasEmMemoria.Where(r => r.ID == regraBolao.IdRegra).FirstOrDefault();

                if (partida.PlacarTimeCasa != "-1")
                {
                    //ClassificacaoEmMemoria classificacaoUsuario = listaClassificacaoEmMemoria.Where(c => c.IdBolao == item.IdBolao && c.IdUsuario == item.IdUsuario).FirstOrDefault();
                    InfoClassificacao info = new InfoClassificacao();
                    info.IdBolao = int.Parse(item.IdBolao);
                    info.IdUsuario = int.Parse(item.IdUsuario);

                    float total = 0;
                    float regra1 = float.Parse(regra.Pontuacao1);
                    float regra2 = float.Parse(regra.Pontuacao2);
                    float regra3 = float.Parse(regra.Pontuacao3);

                    int palpiteTimeCasa = int.Parse(item.PalpiteTimeCasa);
                    int palpiteTimeFora = int.Parse(item.PalpiteTimeFora);

                    int placarTimeCasa = int.Parse(partida.PlacarTimeCasa);
                    int placarTimeFora = int.Parse(partida.PlacarTimeFora);

                    #region Fazer o calculo dos pontos
                    if (placarTimeCasa == palpiteTimeCasa && placarTimeFora == palpiteTimeFora)
                    {
                        total += regra1;
                        info.PlacarCheio = 1;
                    }
                    else if (placarTimeCasa > placarTimeFora)
                    {
                        if (placarTimeCasa == palpiteTimeCasa && placarTimeFora != palpiteTimeFora)
                        {
                            info.PlacarVencedor = 1;
                            total += regra3;
                            if (palpiteTimeCasa > palpiteTimeFora)
                            {
                                info.AcertouVencedor = 1;
                                total += regra2;
                            }
                        }
                        else if (placarTimeCasa != palpiteTimeCasa && placarTimeFora == palpiteTimeFora)
                        {
                            info.PlacarPerdedor = 1;
                            total += regra3;
                            if (palpiteTimeCasa > palpiteTimeFora)
                            {
                                info.AcertouVencedor = 1;
                                total += regra2;
                            }
                        }
                        else if (placarTimeCasa != palpiteTimeCasa && placarTimeFora != palpiteTimeFora && palpiteTimeCasa > palpiteTimeFora)
                        {
                            info.AcertouVencedor = 1;
                            total += regra2;
                        }
                    }
                    else if (placarTimeFora > placarTimeCasa)
                    {
                        if (placarTimeCasa == palpiteTimeCasa && placarTimeFora != palpiteTimeFora)
                        {
                            info.PlacarPerdedor = 1;
                            total += regra3;
                            if (palpiteTimeFora > palpiteTimeCasa)
                            {
                                info.AcertouVencedor = 1;
                                total += regra2;
                            }
                        }
                        else if (placarTimeCasa != palpiteTimeCasa && placarTimeFora == palpiteTimeFora)
                        {
                            info.PlacarVencedor = 1;
                            total += regra3;
                            if (palpiteTimeFora > palpiteTimeCasa)
                            {
                                info.AcertouVencedor = 1;
                                total += regra2;
                            }
                        }
                        else if (placarTimeCasa != palpiteTimeCasa && placarTimeFora != palpiteTimeFora && palpiteTimeFora > palpiteTimeCasa)
                        {
                            info.AcertouVencedor = 1;
                            total += regra2;
                        }
                    }
                    else
                    {
                        if (partida.PlacarTimeCasa == item.PalpiteTimeCasa || partida.PlacarTimeFora == item.PalpiteTimeFora)
                        {
                            total += regra3;
                        }
                        if (palpiteTimeCasa == palpiteTimeFora)
                        {
                            info.AcertouVencedor = 1;
                            total += regra2;
                        }
                    }
                    #endregion

                    info.Total = total;

                    listainfoClassificacao.Add(info);
                    UpdateContabilizadoPalpite(int.Parse(item.ID));
                }
            }

            var query = from p in listainfoClassificacao
                        group p by new { p.IdBolao, p.IdUsuario } into newGroup
                        select new
                        {
                            IdBolao = newGroup.Key.IdBolao,
                            IdUsuario = newGroup.Key.IdUsuario,
                            AcertouVencedor = newGroup.Sum(n => long.Parse(n.AcertouVencedor.ToString())),
                            PlacarCheio = newGroup.Sum(n => long.Parse(n.PlacarCheio.ToString())),
                            PlacarPerdedor = newGroup.Sum(n => long.Parse(n.PlacarPerdedor.ToString())),
                            PlacarVencedor = newGroup.Sum(n => long.Parse(n.PlacarVencedor.ToString())),
                            Total = newGroup.Sum(n => long.Parse(n.Total.ToString())),
                        };

            foreach (var infoSelect in query)
            {
                InfoClassificacao infoClass = new InfoClassificacao()
                {
                    AcertouVencedor = int.Parse(infoSelect.AcertouVencedor.ToString()),
                    IdBolao = int.Parse(infoSelect.IdBolao.ToString()),
                    IdUsuario = int.Parse(infoSelect.IdUsuario.ToString()),
                    PlacarCheio = int.Parse(infoSelect.PlacarCheio.ToString()),
                    PlacarPerdedor = int.Parse(infoSelect.PlacarPerdedor.ToString()),
                    PlacarVencedor = int.Parse(infoSelect.PlacarVencedor.ToString()),
                    Total = int.Parse(infoSelect.Total.ToString())
                };
                if (!VerificaSeClassificacaoExiste(infoClass.IdUsuario.ToString(), infoClass.IdBolao.ToString()))
                {
                    int idEstat = CriarClassificacaoNoBanco(infoClass);
                    classMigradas++;
                }
                else
                {
                    ClassificacaoEmMemoria classificacao = listaClassificacaoEmMemoria.Where(d => int.Parse(d.IdUsuario) == infoClass.IdUsuario && int.Parse(d.IdBolao) == infoClass.IdBolao).FirstOrDefault();
                    infoClass.AcertouVencedor += int.Parse(classificacao.AcertouVencedor);
                    infoClass.PlacarCheio += int.Parse(classificacao.PlacarCheio);
                    infoClass.PlacarPerdedor += int.Parse(classificacao.PlacarPerdedor);
                    infoClass.PlacarVencedor += int.Parse(classificacao.PlacarVencedor);
                    infoClass.PosicaoAnterior = int.Parse(classificacao.Posicao);
                    infoClass.Total += float.Parse(classificacao.Total);
                    infoClass.ID = int.Parse(classificacao.ID);
                    EditarClassificacaoNoBanco(infoClass);
                    classAtt++;
                }

                AtualizarResultado();
            }

            BuscaClassificacaoEmMemoria();

            string idBolaoAnt = "";
            int posicao = 1;
            foreach (var item in listaClassificacaoEmMemoria.OrderByDescending(o => o.IdBolao).ThenByDescending(p => p.Total))
            {
                InfoClassificacao info = new InfoClassificacao();
                if (idBolaoAnt != item.IdBolao)
                {
                    posicao = 1;
                }
                info.ID = int.Parse(item.ID);
                info.Posicao = posicao;
                info.PosicaoAnterior = posicao;
                if (int.Parse(item.Posicao) == 0)
                {
                    info.Variacao = 0;
                }
                else
                {
                    info.Variacao = int.Parse(item.Posicao) - posicao;
                }

                EditarPosicaoClassificacaoNoBanco(info);

                idBolaoAnt = item.IdBolao;

                posicao++;
            }
        }

        public static async Task StartCrawlerResultadosPorLigaasync(List<PartidasEmMemoria> listaPartidasEmMemoria, List<Jogos> jogos, string idPais = "")
        {
            #region Preparando o LoadHtml da página 'Últimos resultados'
            var url = $"https://www.ogol.com.br/ultimos_resultados.php?id_comp_1={idPais}&id_comp_2=0&view=comp";
            WebClient client = new WebClient();
            var data1 = client.DownloadData(url);
            var html = Encoding.Default.GetString(data1);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            //if (!string.IsNullOrEmpty(idPais))
            //{
            //    //Esse código seleciona os jogos por paises
            //    NameValueCollection s = new NameValueCollection();
            //    s.Add("id_pais", idPais);
            //    html = Encoding.Default.GetString(client.UploadValues(url, s));
            //}
            htmlDocument.LoadHtml(html);
            #endregion

            #region Faz a busca do resultado da partida e do Estádio
            var divJogo = htmlDocument.DocumentNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("parent")).ToList();

            foreach (var div in divJogo)
            {
                var jogo = new Jogos();
                var infoPartida = div.Descendants("td").ToList();
                int placarCasa = -1;
                int placarFora = -1;

                if (infoPartida[4].InnerText.Contains("Pen"))
                {
                    placarCasa = int.Parse(infoPartida[4].InnerText.Split('-')[0]);
                    placarFora = int.Parse(infoPartida[4].InnerText.Split('-')[1].Split('(')[0]);
                }
                else
                {
                    placarCasa = int.Parse(infoPartida[4].InnerText.Split('-')[0]);
                    placarFora = int.Parse(infoPartida[4].InnerText.Split('-')[1]);
                }
                var partida = BuscaIdPartidaPeloIdConversao(listaPartidasEmMemoria, div.Id);
                if (partida != null)
                {
                    jogo.IdPartida = partida.IdConversao;
                    jogo.PlacarCasa = placarCasa;
                    jogo.PlacarFora = placarFora;
                }
                else
                {
                    jogo = PreencheInfoJogos(div, placarCasa, placarFora, div.Id);

                    if (jogo.TimeCasa.Contains("/") || jogo.TimeFora.Contains("/"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Sub-18"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("U20"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Woman"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Feminina"))
                    {
                        continue;
                    }
                    if (jogo.TimeCasa.Contains("Derrotado MF 1") || jogo.TimeCasa.Contains("Vencedor MF 1") || jogo.TimeCasa.Contains("Vencedor MF 2") || jogo.TimeFora.Contains("Vencedor MF 2"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Sul-Mt.Grossense Série B"))
                    {
                        continue;
                    }
                }

                jogos.Add(jogo);
            }
            #endregion
        }

        public static async Task StartCrawlerResultadosPorPaisasync(List<PartidasEmMemoria> listaPartidasEmMemoria, List<Jogos> jogos, string idPais = "")
        {
            #region Preparando o LoadHtml da página 'Últimos resultados'
            var url = $"https://www.ogol.com.br/ultimos_resultados.php";
            WebClient client = new WebClient();
            var data1 = client.DownloadData(url);
            var html = Encoding.Default.GetString(data1);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            if (!string.IsNullOrEmpty(idPais))
            {
                //Esse código seleciona os jogos por paises
                NameValueCollection s = new NameValueCollection();
                s.Add("id_pais", idPais);
                html = Encoding.Default.GetString(client.UploadValues(url, s));
            }
            htmlDocument.LoadHtml(html);
            #endregion

            #region Faz a busca do resultado da partida e do Estádio
            var divJogo = htmlDocument.DocumentNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("parent")).ToList();

            foreach (var div in divJogo)
            {
                var jogo = new Jogos();
                var infoPartida = div.Descendants("td").ToList();
                int placarCasa = -1;
                int placarFora = -1;

                if (infoPartida[4].InnerText.Contains("Pen"))
                {
                    placarCasa = int.Parse(infoPartida[4].InnerText.Split('-')[0]);
                    placarFora = int.Parse(infoPartida[4].InnerText.Split('-')[1].Split('(')[0]);
                }
                else
                {
                    placarCasa = int.Parse(infoPartida[4].InnerText.Split('-')[0]);
                    placarFora = int.Parse(infoPartida[4].InnerText.Split('-')[1]);
                }
                var partida = BuscaIdPartidaPeloIdConversao(listaPartidasEmMemoria, div.Id);
                if (partida != null)
                {
                    jogo.IdPartida = partida.IdConversao;
                    jogo.PlacarCasa = placarCasa;
                    jogo.PlacarFora = placarFora;
                }
                else
                {
                    jogo = PreencheInfoJogos(div, placarCasa, placarFora, div.Id);

                    if (jogo.TimeCasa.Contains("/") || jogo.TimeFora.Contains("/"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Sub-18"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("U20"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Woman"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Feminina"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Feminino"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("PL2 Div."))
                    {
                        continue;
                    }
                    if (jogo.TimeCasa.Contains("Derrotado MF 1") || jogo.TimeCasa.Contains("Vencedor MF 1") || jogo.TimeCasa.Contains("Vencedor MF 2") || jogo.TimeFora.Contains("Vencedor MF 2"))
                    {
                        continue;
                    }
                    if (jogo.Campeonato.Contains("Sul-Mt.Grossense Série B"))
                    {
                        continue;
                    }
                }

                jogos.Add(jogo);
            }
            #endregion
        }

        private static PartidasEmMemoria BuscaIdPartidaPeloIdConversao(List<PartidasEmMemoria> listaPartidasEmMemoria, string idConversao)
        {
            return listaPartidasEmMemoria.Where(p => p.IdConversao == idConversao).FirstOrDefault();
        }

        private static async Task StartCrawlerasync(List<Jogos> jogos)
        {
            #region Preparando o LoadHtml da página 'Próximos jogos'
            var url = "https://www.ogol.com.br/proximos_jogos.php";
            WebClient client = new WebClient();

            var data1 = client.DownloadData(url);
            var html = Encoding.Default.GetString(data1);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);
            #endregion

            #region Faz a busca das partidas e Adicina na lista de Jogos
            var divs = htmlDocument.DocumentNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("parent")).ToList();

            foreach (var div in divs)
            {
                var jogo = PreencheInfoJogos(div, -1, -1, div.Id);

                if (jogo.TimeCasa.Contains("/") || jogo.TimeFora.Contains("/"))
                {
                    continue;
                }
                if (jogo.Campeonato.Contains("CAF U"))
                {
                    continue;
                }
                if (jogo.Campeonato.Contains("Sub-18"))
                {
                    continue;
                }
                if (jogo.Campeonato.Contains("U20"))
                {
                    continue;
                }
                if (jogo.Campeonato.Contains("Feminino"))
                {
                    continue;
                }
                if (jogo.Campeonato.Contains("Fem."))
                {
                    continue;
                }
                if (jogo.TimeCasa.Contains("Derrotado MF 1") || jogo.TimeCasa.Contains("Vencedor MF 1") || jogo.TimeCasa.Contains("Vencedor MF 2") || jogo.TimeFora.Contains("Vencedor MF 2"))
                {
                    continue;
                }

                jogos.Add(jogo);
            }
            #endregion
        }

        private static async Task StartCrawlerasyncNBA(List<Jogos> jogos)
        {
            #region Preparando o LoadHtml da página
            var url = "https://www.espn.com.br/nba/calendario";
            WebClient client = new WebClient();
            var data1 = client.DownloadData(url);
            var html = Encoding.Default.GetString(data1);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);
            #endregion

            #region Faz a busca das partidas e adiciona na lista de Jogos
            var divs = htmlDocument.DocumentNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("odd") || node.GetAttributeValue("class", "").Equals("even")).ToList();

            foreach (var div in divs)
            {
                Jogos jogo = new Jogos();

                var infoPartida = div.Descendants("td").ToList();

                var nomeTimes = div.Descendants("abbr").ToList();

                if (infoPartida[2].InnerText == "Cancelado")
                {
                    continue;
                }

                string[] separator = new string[] { "jogoId=" };

                if (infoPartida[2].Attributes["data-date"] == null)
                {
                    continue;
                }

                jogo.TimeCasa = nomeTimes[1].GetAttributeValue("title", "");
                jogo.TimeFora = nomeTimes[0].GetAttributeValue("title", "");
                jogo.Data = DateTimeOffset.Parse(infoPartida[2].Attributes["data-date"].Value).LocalDateTime;
                string idPartida = $"NBA{infoPartida[2].Descendants("a").ToList()[0].GetAttributeValue("href", "").Split(separator, StringSplitOptions.None)[1]}";
                jogo.IdPartida = idPartida;
                jogo.Campeonato = "30";
                jogo.Rodada = "40";
                jogo.PlacarCasa = -1;
                jogo.PlacarFora = -1;

                jogos.Add(jogo);
            }
            #endregion
        }

        private void CrawlerProximasPartidasNBA()
        {
            var jogos = new List<Jogos>();

            StartCrawlerasyncNBA(jogos);

            foreach (Jogos jogo in jogos)
            {
                const string modalidade = "2";
                string idTimeCasa = RetornaIDTime(jogo.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(jogo.TimeFora, modalidade);
                string idPaisFora;
                string idPaisCasa;
                if (idTimeCasa == "117")
                {
                    idPaisCasa = "3";
                }
                else
                {
                    idPaisCasa = "2";
                }
                if (idTimeFora == "117")
                {
                    idPaisFora = "3";
                }
                else
                {
                    idPaisFora = "2";
                }

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    jogo.TimeCasa = CriarTime(jogo.TimeCasa, idPaisCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    jogo.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    jogo.TimeFora = CriarTime(jogo.TimeFora, idPaisFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    jogo.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(jogo.TimeCasa, jogo.TimeFora, jogo.Data.ToString()))
                {
                    int idPartida = CriarPartida(jogo);
                    partidasMigradas++;

                    if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), jogo.Rodada))
                    {
                        CriarRodadaPartida(idPartida.ToString(), jogo.Rodada);
                        rodadapartidasMigradas++;
                    }
                }
                else
                {
                    partidasJaExistem++;
                }

                AtualizarResultado();
            }
        }

        private static async Task StartCrawlerasyncResultadosNBA(List<Jogos> jogos, string url, SortedList<string, string> abreviaturaTimesNBA, List<InfoJogosAtrasadosNBA> listaJogosNBA)
        {
            #region Preparando o LoadHtml da página
            WebClient client = new WebClient();
            var data1 = client.DownloadData(url);
            var html = Encoding.Default.GetString(data1);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);
            #endregion

            #region Faz a busca das partidas e adiciona na lista de Jogos
            var divs = htmlDocument.DocumentNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("odd") || node.GetAttributeValue("class", "").Equals("even")).ToList();
            var teste = listaJogosNBA.Where(d => d.URL == url).FirstOrDefault();
            int j = 1;

            foreach (var div in divs)
            {
                Jogos jogo = new Jogos();

                var infoPartida = div.Descendants("td").ToList();

                var nomeTimes = div.Descendants("abbr").ToList();

                if (infoPartida[2].InnerText == "Cancelado")
                {
                    j++;
                    continue;
                }

                var placar = infoPartida[2].InnerText.Split(',');

                string[] separator = new string[] { "jogoId=" };

                jogo.TimeCasa = nomeTimes[1].GetAttributeValue("title", "");
                jogo.TimeFora = nomeTimes[0].GetAttributeValue("title", "");

                for (int i = 0; i < teste.Datas.Count; i++)
                {
                    if (j <= teste.PartidasPorDia[i])
                    {
                        jogo.Data = Convert.ToDateTime(teste.Datas[i]);
                        break;
                    }
                }

                string idPartida = infoPartida[2].Descendants("a").ToList()[0].GetAttributeValue("href", "").Split(separator, StringSplitOptions.None)[1];
                jogo.IdPartida = $"NBA{idPartida}";
                jogo.Campeonato = "30";
                jogo.Rodada = "40";

                #region Busca placar dos times
                if (jogo.TimeCasa == abreviaturaTimesNBA[placar[0].TrimEnd().TrimStart().Split(' ')[0]])
                {
                    jogo.PlacarCasa = int.Parse(placar[0].TrimEnd().TrimStart().Split(' ')[1]);
                    jogo.PlacarFora = int.Parse(placar[1].TrimEnd().TrimStart().Split(' ')[1]);
                }
                else
                {
                    jogo.PlacarFora = int.Parse(placar[0].TrimEnd().TrimStart().Split(' ')[1]);
                    jogo.PlacarCasa = int.Parse(placar[1].TrimEnd().TrimStart().Split(' ')[1]);
                }
                #endregion

                jogos.Add(jogo);
                j++;
            }
            #endregion
        }

        public List<InfoJogosAtrasadosNBA> PreencheInfoJogosNBA()
        {
            List<InfoJogosAtrasadosNBA> ListaJogosNBAtrados = new List<InfoJogosAtrasadosNBA>();

            InfoJogosAtrasadosNBA info = new InfoJogosAtrasadosNBA();
            info.Datas = new List<string>() {
                "02/03/2021",
                "03/02/2021",
                "04/03/2021",
                "07/03/2021"
            };
            info.PartidasPorDia = new List<int>() {
                7, 17, 26, 27
            };
            info.URL = "https://www.espn.com.br/nba/calendario/_/data/20210302";
            ListaJogosNBAtrados.Add(info);

            return ListaJogosNBAtrados;
        }

        private void CrawlerResultadosPartidasNBA()
        {
            List<string> urlPartidasPorSemana = new List<string>()
            {
                "https://www.espn.com.br/nba/calendario/_/data/20210302"
            };

            SortedList<string, string> abreviaturaTimeNBA = new SortedList<string, string>()
            {
                { "ATL", "Atlanta Hawks" },
                { "BOS", "Boston Celtics" },
                { "BKN", "Brooklyn Nets" },
                { "CHA", "Charlotte Hornets" },
                { "CHI", "Chicago Bulls" },
                { "CLE", "Cleveland Cavaliers" },
                { "DAL", "Dallas Mavericks" },
                { "DEN", "Denver Nuggets" },
                { "DET", "Detroit Pistons" },
                { "GS", "Golden State Warriors" },
                { "HOU", "Houston Rockets" },
                { "IND", "Indiana Pacers" },
                { "LAC", "LA Clippers" },
                { "LAL", "Los Angeles Lakers" },
                { "MEM", "Memphis Grizzlies" },
                { "MIA", "Miami Heat" },
                { "MIL", "Milwaukee Bucks" },
                { "MIN", "Minnesota Timberwolves" },
                { "NO", "New Orleans Pelicans" },
                { "NY", "New York Knicks" },
                { "OKC", "Oklahoma City Thunder" },
                { "ORL", "Orlando Magic" },
                { "PHI", "Philadelphia 76ers" },
                { "PHX", "Phoenix Suns" },
                { "POR", "Portland Trail Blazers" },
                { "SAC", "Sacramento Kings" },
                { "SA", "San Antonio Spurs" },
                { "TOR", "Toronto Raptors" },
                { "UTAH", "Utah Jazz" },
                { "WSH", "Washington Wizards" }
            };

            var listaJogosNBA = PreencheInfoJogosNBA();

            var jogos = new List<Jogos>();

            foreach (string url in urlPartidasPorSemana)
            {
                StartCrawlerasyncResultadosNBA(jogos, url, abreviaturaTimeNBA, listaJogosNBA);
            }

            foreach (Jogos jogo in jogos)
            {
                if (VerificaSePartidaExisteIdConversao(jogo.IdPartida))
                {
                    using (SqlConnection connection = new SqlConnection(tbConexao.Text))
                    {
                        connection.Open();
                        string sql = "update Partida set placarTimeCasa = @placarTimeCasa,placarTimeFora = @placarTimeFora where idConversao = @idConversao";
                        using (SqlCommand cmd = new SqlCommand(sql, connection))
                        {
                            cmd.Parameters.Add("@idConversao", SqlDbType.VarChar).Value = jogo.IdPartida.ToString();
                            cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = jogo.PlacarCasa;
                            cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = jogo.PlacarFora;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            partidasAtt++;
                        }
                    }
                }
                else
                {
                    const string modalidade = "2";
                    string idTimeCasa = RetornaIDTime(jogo.TimeCasa, modalidade);
                    string idTimeFora = RetornaIDTime(jogo.TimeFora, modalidade);
                    string idPaisFora;
                    string idPaisCasa;
                    if (idTimeCasa == "117")
                    {
                        idPaisCasa = "3";
                    }
                    else
                    {
                        idPaisCasa = "2";
                    }
                    if (idTimeFora == "117")
                    {
                        idPaisFora = "3";
                    }
                    else
                    {
                        idPaisFora = "2";
                    }

                    #region Verifica Time da Casa
                    if (string.IsNullOrEmpty(idTimeCasa))
                    {
                        jogo.TimeCasa = CriarTime(jogo.TimeCasa, idPaisCasa, modalidade).ToString();
                        timesMigrados++;
                    }
                    else
                        jogo.TimeCasa = idTimeCasa;
                    #endregion

                    #region Verifica Time da Fora
                    if (string.IsNullOrEmpty(idTimeFora))
                    {
                        jogo.TimeFora = CriarTime(jogo.TimeFora, idPaisFora, modalidade).ToString();
                        timesMigrados++;
                    }
                    else
                        jogo.TimeFora = idTimeFora;
                    #endregion

                    if (!VerificaSePartidaExiste(jogo.TimeCasa, jogo.TimeFora, jogo.Data.ToString()))
                    {
                        int idPartida = CriarPartida(jogo);
                        partidasMigradas++;

                        if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), jogo.Rodada))
                        {
                            CriarRodadaPartida(idPartida.ToString(), jogo.Rodada);
                            rodadapartidasMigradas++;
                        }
                    }
                    else
                    {
                        partidasJaExistem++;
                    }
                }
            }
            AtualizarResultado();
        }

        private static async Task StartCrawlerasyncNBB(List<Jogos> jogos)
        {
            #region Preparando o LoadHtml da página
            var url = "https://lnb.com.br/nbb/tabela-de-jogos/";
            WebClient client = new WebClient();
            var data1 = client.DownloadData(url);
            var html = Encoding.Default.GetString(data1);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);
            #endregion

            #region Faz a busca das partidas e adiciona na lista de Jogos
            var divs = htmlDocument.DocumentNode.Descendants("tr").ToList();

            foreach (var div in divs)
            {
                try
                {
                    Jogos jogo = new Jogos();

                    var infoPartida = div.Descendants("td").ToList();

                    if (infoPartida.Count == 0)
                    {
                        continue;
                    }

                    var dataSeparada = infoPartida[1].InnerText.TrimStart().TrimEnd().Replace("\r\n", "").Replace("                                        ", "-").Split('-')[0].Split('/').ToList();
                    var horaSeparada = infoPartida[1].InnerText.TrimStart().TrimEnd().Replace("\r\n", "").Replace("                                        ", "-").Split('-')[1].Split(':').ToList();

                    DateTime data = new DateTime(int.Parse(dataSeparada[2]), int.Parse(dataSeparada[1]), int.Parse(dataSeparada[0]), int.Parse(horaSeparada[0]), int.Parse(horaSeparada[1]), 0);

                    jogo.TimeCasa = infoPartida[3].InnerText.TrimStart().TrimEnd();
                    if (jogo.TimeCasa == "SÃ£o Paulo")
                    {
                        jogo.TimeCasa = "São Paulo";
                    }
                    if (jogo.TimeCasa == "BrasÃ­lia" || jogo.TimeCasa == "BRB/BrasÃ­lia")
                    {
                        jogo.TimeCasa = "Brasília";
                    }
                    jogo.TimeFora = infoPartida[7].InnerText.TrimStart().TrimEnd();
                    if (jogo.TimeFora == "SÃ£o Paulo")
                    {
                        jogo.TimeFora = "São Paulo";
                    }
                    if (jogo.TimeFora == "BrasÃ­lia" || jogo.TimeFora == "BRB/BrasÃ­lia")
                    {
                        jogo.TimeFora = "Brasília";
                    }
                    jogo.Data = data;
                    string idPartida = infoPartida[0].GetAttributeValue("data-real-id", "");
                    jogo.IdPartida = $"NBB{idPartida}";
                    jogo.Campeonato = "31";
                    jogo.Rodada = infoPartida[9].InnerText.Replace("Âª", "ª");
                    if (infoPartida[5].InnerText.TrimEnd().TrimStart().Replace("\r\n", "").Replace("                                            ", "").StartsWith("X"))
                    {
                        jogo.PlacarCasa = -1;
                        jogo.PlacarFora = -1;
                    }
                    else
                    {
                        jogo.PlacarCasa = int.Parse(infoPartida[5].InnerText.TrimEnd().TrimStart().Replace("\r\n", "").Replace("                                            ", "").Split(' ')[0].Split('X')[0]);
                        jogo.PlacarFora = int.Parse(infoPartida[5].InnerText.TrimEnd().TrimStart().Replace("\r\n", "").Replace("                                            ", "").Split(' ')[0].Split('X')[1]);
                    }

                    jogos.Add(jogo);
                }
                catch (Exception e)
                {

                    throw;
                }
            }
            #endregion
        }

        private void CrawlerProximasPartidasNBB()
        {
            var jogos = new List<Jogos>();

            StartCrawlerasyncNBB(jogos);

            foreach (Jogos jogo in jogos)
            {
                if (VerificaSePartidaExisteIdConversao(jogo.IdPartida))
                {
                    using (SqlConnection connection = new SqlConnection(tbConexao.Text))
                    {
                        connection.Open();
                        string sql = "update Partida set placarTimeCasa = @placarTimeCasa,placarTimeFora = @placarTimeFora where idConversao = @idConversao";
                        using (SqlCommand cmd = new SqlCommand(sql, connection))
                        {
                            cmd.Parameters.Add("@idConversao", SqlDbType.VarChar).Value = jogo.IdPartida.ToString();
                            cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = jogo.PlacarCasa;
                            cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = jogo.PlacarFora;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            partidasAtt++;
                        }
                    }
                }
                else
                {
                    const string modalidade = "2";
                    string idTimeCasa = RetornaIDTime(jogo.TimeCasa, modalidade);
                    string idTimeFora = RetornaIDTime(jogo.TimeFora, modalidade);
                    string idRodada = RetornaIDRodada(jogo.Rodada);
                    string idPais = "1";

                    #region Verifica Rodada
                    if (string.IsNullOrEmpty(idRodada))
                    {
                        string nomeRodada = jogo.Rodada;
                        jogo.Rodada = CriarRodada(nomeRodada).ToString();
                        idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), jogo.Rodada);
                        rodadasMigradas++;
                    }
                    else
                        jogo.Rodada = idRodada;
                    #endregion

                    #region Verifica Time da Casa
                    if (string.IsNullOrEmpty(idTimeCasa))
                    {
                        jogo.TimeCasa = CriarTime(jogo.TimeCasa, idPais, modalidade).ToString();
                        timesMigrados++;
                    }
                    else
                        jogo.TimeCasa = idTimeCasa;
                    #endregion

                    #region Verifica Time da Fora
                    if (string.IsNullOrEmpty(idTimeFora))
                    {
                        jogo.TimeFora = CriarTime(jogo.TimeFora, idPais, modalidade).ToString();
                        timesMigrados++;
                    }
                    else
                        jogo.TimeFora = idTimeFora;
                    #endregion

                    if (!VerificaSePartidaExiste(jogo.TimeCasa, jogo.TimeFora, jogo.Data.ToString()))
                    {
                        int idPartida = CriarPartida(jogo);
                        partidasMigradas++;

                        if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), jogo.Rodada))
                        {
                            CriarRodadaPartida(idPartida.ToString(), jogo.Rodada);
                            rodadapartidasMigradas++;
                        }
                    }
                    else
                    {
                        partidasJaExistem++;
                    }
                }

                AtualizarResultado();
            }
        }

        private void CrawlerBuscarProximasPartidas()
        {
            var jogos = new List<Jogos>();

            StartCrawlerasync(jogos);

            foreach (Jogos jogo in jogos)
            {
                const string modalidade = "1";
                string idTimeCasa = RetornaIDTime(jogo.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(jogo.TimeFora, modalidade);
                string idRodada = RetornaIDRodada(jogo.Rodada);
                string idCampeonato = RetornaIDCampeonato(jogo.Campeonato);
                string idPaisTimeCasa = RetornaIDPais(jogo.NomePaisTimeCasa);
                string idPaisTimeFora = RetornaIDPais(jogo.NomePaisTimeFora);

                #region Verifica Campeonato
                if (string.IsNullOrEmpty(idCampeonato))
                {
                    string nomeCampeonato = jogo.Campeonato;
                    jogo.Campeonato = CriarCampeonato(nomeCampeonato).ToString();
                    campeonatosPorNome.Add(RemoveAccents(nomeCampeonato).ToUpper(), jogo.Campeonato);
                    campeonatosMigrados++;
                }
                else
                    jogo.Campeonato = idCampeonato;
                #endregion

                #region Verifica Rodada
                if (string.IsNullOrEmpty(idRodada))
                {
                    string nomeRodada = jogo.Rodada;
                    jogo.Rodada = CriarRodada(nomeRodada).ToString();
                    idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), jogo.Rodada);
                    rodadasMigradas++;
                }
                else
                    jogo.Rodada = idRodada;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    jogo.TimeCasa = CriarTime(jogo.TimeCasa, idPaisTimeCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    jogo.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    jogo.TimeFora = CriarTime(jogo.TimeFora, idPaisTimeFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    jogo.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(jogo.TimeCasa, jogo.TimeFora, jogo.Data.ToString()))
                {
                    int idPartida = CriarPartida(jogo);
                    partidasMigradas++;

                    if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), jogo.Rodada))
                    {
                        CriarRodadaPartida(idPartida.ToString(), jogo.Rodada);
                        rodadapartidasMigradas++;
                    }
                }
                else
                {
                    partidasJaExistem++;
                }

                AtualizarResultado();
            }
        }

        private void CrawlerResultadosPartidasPorPais()
        {
            SortedList<string, string> listaPaises = new SortedList<string, string>() {
                { "1", "Portugal" },
                //{ "2", "Alemanha" },
                //{ "4", "Argentina" },
                //{ "6", "Brasil" },
                //{ "12", "Espanha" },
                //{ "14", "França" },
                //{ "15", "Holanda" },
                //{ "16", "Inglaterra" },
                //{ "18", "Itália" }
            };

            foreach (KeyValuePair<string, string> pais in listaPaises)
            {
                var jogos = new List<Jogos>();

                StartCrawlerResultadosPorPaisasync(listaPartidasEmMemoria, jogos, pais.Key);

                foreach (var jogo in jogos)
                {
                    if (VerificaSePartidaExisteIdConversao(jogo.IdPartida))
                    {
                        using (SqlConnection connection = new SqlConnection(tbConexao.Text))
                        {
                            connection.Open();
                            string sql = "update Partida set placarTimeCasa = @placarTimeCasa,placarTimeFora = @placarTimeFora where idConversao = @idConversao";
                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                cmd.Parameters.Add("@idConversao", SqlDbType.VarChar).Value = jogo.IdPartida.ToString();
                                cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = jogo.PlacarCasa;
                                cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = jogo.PlacarFora;
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                                partidasAtt++;
                            }
                        }
                    }
                    else
                    {
                        const string modalidade = "1";
                        string idTimeCasa = RetornaIDTime(jogo.TimeCasa, modalidade);
                        string idTimeFora = RetornaIDTime(jogo.TimeFora, modalidade);
                        string idRodada = RetornaIDRodada(jogo.Rodada);
                        string idCampeonato = RetornaIDCampeonato(jogo.Campeonato);
                        string idPaisTimeCasa = RetornaIDPais(jogo.NomePaisTimeCasa);
                        string idPaisTimeFora = RetornaIDPais(jogo.NomePaisTimeFora);

                        #region Verifica Campeonato
                        if (string.IsNullOrEmpty(idCampeonato))
                        {
                            string nomeCampeonato = jogo.Campeonato;
                            jogo.Campeonato = CriarCampeonato(nomeCampeonato).ToString();
                            campeonatosPorNome.Add(RemoveAccents(nomeCampeonato).ToUpper(), jogo.Campeonato);
                            campeonatosMigrados++;
                        }
                        else
                            jogo.Campeonato = idCampeonato;
                        #endregion

                        #region Verifica Rodada
                        if (string.IsNullOrEmpty(idRodada))
                        {
                            string nomeRodada = jogo.Rodada;
                            jogo.Rodada = CriarRodada(nomeRodada).ToString();
                            idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), jogo.Rodada);
                            rodadasMigradas++;
                        }
                        else
                            jogo.Rodada = idRodada;
                        #endregion

                        #region Verifica Time da Casa
                        if (string.IsNullOrEmpty(idTimeCasa))
                        {
                            jogo.TimeCasa = CriarTime(jogo.TimeCasa, idPaisTimeCasa, modalidade).ToString();
                            timesMigrados++;
                        }
                        else
                            jogo.TimeCasa = idTimeCasa;
                        #endregion

                        #region Verifica Time da Fora
                        if (string.IsNullOrEmpty(idTimeFora))
                        {
                            jogo.TimeFora = CriarTime(jogo.TimeFora, idPaisTimeFora, modalidade).ToString();
                            timesMigrados++;
                        }
                        else
                            jogo.TimeFora = idTimeFora;
                        #endregion

                        if (!VerificaSePartidaExiste(jogo.TimeCasa, jogo.TimeFora, jogo.Data.ToString()))
                        {
                            int idPartida = CriarPartida(jogo);
                            partidasMigradas++;

                            if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), jogo.Rodada))
                            {
                                CriarRodadaPartida(idPartida.ToString(), jogo.Rodada);
                                rodadapartidasMigradas++;
                            }
                        }
                        else
                        {
                            partidasJaExistem++;
                        }
                    }

                    AtualizarResultado();
                }
            }
        }

        private void CrawlerResultadosPartidasPorLiga()
        {
            SortedList<string, string> listaLigas = new SortedList<string, string>() {
                { "27", "Champions League" },
                //{ "144", "Mundial de Clubes" },
                //{ "28", "Europa League" },
                //{ "34", "Supercopa Europeia" },
                //{ "58", "Libertadores" },
                //{ "269", "Sulamericana" },
                //{ "268", "Recopa" }
            };

            foreach (KeyValuePair<string, string> pais in listaLigas)
            {
                var jogos = new List<Jogos>();

                StartCrawlerResultadosPorLigaasync(listaPartidasEmMemoria, jogos, pais.Key);

                foreach (var jogo in jogos)
                {
                    if (VerificaSePartidaExisteIdConversao(jogo.IdPartida))
                    {
                        using (SqlConnection connection = new SqlConnection(tbConexao.Text))
                        {
                            connection.Open();
                            string sql = "update Partida set placarTimeCasa = @placarTimeCasa,placarTimeFora = @placarTimeFora where idConversao = @idConversao";
                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                cmd.Parameters.Add("@idConversao", SqlDbType.VarChar).Value = jogo.IdPartida.ToString();
                                cmd.Parameters.Add("@placarTimeCasa", SqlDbType.Int).Value = jogo.PlacarCasa;
                                cmd.Parameters.Add("@placarTimeFora", SqlDbType.Int).Value = jogo.PlacarFora;
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                                partidasAtt++;
                            }
                        }
                    }
                    else
                    {
                        const string modalidade = "1";
                        string idTimeCasa = RetornaIDTime(jogo.TimeCasa, modalidade);
                        string idTimeFora = RetornaIDTime(jogo.TimeFora, modalidade);
                        string idRodada = RetornaIDRodada(jogo.Rodada);
                        string idCampeonato = RetornaIDCampeonato(jogo.Campeonato);
                        string idPaisTimeCasa = RetornaIDPais(jogo.NomePaisTimeCasa);
                        string idPaisTimeFora = RetornaIDPais(jogo.NomePaisTimeFora);

                        #region Verifica Campeonato
                        if (string.IsNullOrEmpty(idCampeonato))
                        {
                            string nomeCampeonato = jogo.Campeonato;
                            jogo.Campeonato = CriarCampeonato(nomeCampeonato).ToString();
                            campeonatosPorNome.Add(RemoveAccents(nomeCampeonato).ToUpper(), jogo.Campeonato);
                            campeonatosMigrados++;
                        }
                        else
                            jogo.Campeonato = idCampeonato;
                        #endregion

                        #region Verifica Rodada
                        if (string.IsNullOrEmpty(idRodada))
                        {
                            string nomeRodada = jogo.Rodada;
                            jogo.Rodada = CriarRodada(nomeRodada).ToString();
                            idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), jogo.Rodada);
                            rodadasMigradas++;
                        }
                        else
                            jogo.Rodada = idRodada;
                        #endregion

                        #region Verifica Time da Casa
                        if (string.IsNullOrEmpty(idTimeCasa))
                        {
                            jogo.TimeCasa = CriarTime(jogo.TimeCasa, idPaisTimeCasa, modalidade).ToString();
                            timesMigrados++;
                        }
                        else
                            jogo.TimeCasa = idTimeCasa;
                        #endregion

                        #region Verifica Time da Fora
                        if (string.IsNullOrEmpty(idTimeFora))
                        {
                            jogo.TimeFora = CriarTime(jogo.TimeFora, idPaisTimeFora, modalidade).ToString();
                            timesMigrados++;
                        }
                        else
                            jogo.TimeFora = idTimeFora;
                        #endregion

                        if (!VerificaSePartidaExiste(jogo.TimeCasa, jogo.TimeFora, jogo.Data.ToString()))
                        {
                            int idPartida = CriarPartida(jogo);
                            partidasMigradas++;

                            if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), jogo.Rodada))
                            {
                                CriarRodadaPartida(idPartida.ToString(), jogo.Rodada);
                                rodadapartidasMigradas++;
                            }
                        }
                        else
                        {
                            partidasJaExistem++;
                        }
                    }

                    AtualizarResultado();
                }
            }
        }

        private static Jogos PreencheInfoJogos(HtmlNode div, int placarTimeCasa, int placarTimeFora, string idPartida)
        {
            SortedList<string, string> nomesRodadas = new SortedList<string, string>() {
                {"Grp.", "Fase de Grupos"},
                {"POff", "Playoff"},
                {"1F", "Primeira Fase"},
                {"2F", "Segunda Fase"},
                {"&nbsp;", "Fase de Grupos"},
                {"5E", "5ª Eliminatória"},
                {"4E", "4ª Eliminatória"},
                {"3E", "3ª Eliminatória"},
                {"2E", "2ª Eliminatória"},
                {"1E", "1ª Eliminatória"},
                {"QF", "Quartas de Final"},
                {"SF", "Semi-Final"},
                {"F", "Final"},
                {"3/4", "Terceiro Lugar"},
                {"1/8", "Oitavas de Final"},
                {"1/4", "Quartas de Final"}
            };

            SortedList<string, string> nomesCampeonatosParaAtt = new SortedList<string, string>() {
                {"UEFA Nations League", "UEFA Nations League"},
                {"Elim. Copa do Mundo (CONMEBOL)", "Eliminatórias - Copa do Mundo"},
                {"Brasileirão", "Brasileirão"},
                {"Amistosos Seleções", "Amistoso"},
                {"Brasileirão - Série D", "Brasileirão - Série D"},
                {"Brasileirão - Série C", "Brasileirão - Série C"},
                {"Brasileirão - Série B", "Brasileirão - Série B"},
                {"Copa do Brasil Sub-20", "Copa do Brasil Sub-20"},
                {"Euro (E)", "Eliminatórias - Eurocopa"},
                {"Liga Paraguaia", "Campeonato Paraguaio"},
                {"KNVB-beker", "Copa da Holanda"},
                {"Coupe de France", "Copa da França"},
                {"1. Bundesliga", "Bundesliga" },
                {"Russia 2020/21", "Campeonato Russo" },
                {"Schweizer Pokal 20/201", "Schweizer Pokal" },
                {"Austria", "Campeonato Austríaco" },
                {"Holland", "Campeonato Holandês" },
                {"Nordestão", "Copa do Nordeste" },
                {"Nordeste 21", "Copa do Nordeste" }
            };

            Jogos jogo = new Jogos();

            var infoPartida = div.Descendants("td").ToList();
            var dataSeparada = infoPartida[1].InnerText.Split('-');
            string[] horaSeparada;
            if (!string.IsNullOrEmpty(infoPartida[2].InnerText))
            {
                horaSeparada = infoPartida[2].InnerText.Split(':');
            }
            else
            {
                horaSeparada = new string[] { "0", "0" };
            }
            DateTime data = new DateTime(Convert.ToInt32(dataSeparada[0]),
                                         Convert.ToInt32(dataSeparada[1]),
                                         Convert.ToInt32(dataSeparada[2]),
                                         Convert.ToInt32(horaSeparada[0]),
                                         Convert.ToInt32(horaSeparada[1]), 0);
            string fase = infoPartida[6].InnerText;
            string nomePaisCasa = "";
            string nomePaisFora = "";

            if (infoPartida[3].InnerHtml.Contains("title=\""))
            {
                nomePaisCasa = infoPartida[3].InnerHtml.Split(new string[] { "title=\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"" }, StringSplitOptions.None)[0];
            }
            else if (infoPartida[7].InnerHtml.Contains("title=\""))
            {
                nomePaisCasa = infoPartida[7].InnerHtml.Split(new string[] { "title=\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"" }, StringSplitOptions.None)[0];
            }
            if (infoPartida[5].InnerHtml.Contains("title=\""))
            {
                nomePaisFora = infoPartida[5].InnerHtml.Split(new string[] { "title=\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"" }, StringSplitOptions.None)[0];
            }
            else if (infoPartida[7].InnerHtml.Contains("title=\""))
            {
                nomePaisFora = infoPartida[7].InnerHtml.Split(new string[] { "title=\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"" }, StringSplitOptions.None)[0];
            }

            if (fase.Contains("Grp."))
            {
                fase = "Fase de Grupos";
            }
            else
            {
                if (fase.Contains("R"))
                {
                    for (int i = 1; i <= 50; i++)
                    {
                        if (fase == $"R{i}")
                        {
                            fase = $"{i}ª Rodada";
                            break;
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, string> item in nomesRodadas)
            {
                if (fase == item.Key)
                {
                    fase = item.Value;
                }
            }

            string competicao = infoPartida[7].InnerText;

            if (competicao.Contains("2020/21"))
            {
                competicao = competicao.Replace("2020/21", "").TrimEnd();
            }
            else if (competicao.Contains("20/21"))
            {
                competicao = competicao.Replace("20/21", "").TrimEnd();
            }
            else if (competicao.Contains("2020"))
            {
                competicao = competicao.Replace("2020", "").TrimEnd();
            }
            else if (competicao.Contains("2021"))
            {
                competicao = competicao.Replace("2021", "").TrimEnd();
            }
            else if (competicao.Contains("2019"))
            {
                competicao = competicao.Replace("2019", "").TrimEnd();
            }
            else if (competicao.Contains("2018"))
            {
                competicao = competicao.Replace("2018", "").TrimEnd();
            }
            else if (competicao.Contains("2017"))
            {
                competicao = competicao.Replace("2017", "").TrimEnd();
            }
            else if (competicao.Contains("2016"))
            {
                competicao = competicao.Replace("2016", "").TrimEnd();
            }
            else if (competicao.Contains("2015"))
            {
                competicao = competicao.Replace("2015", "").TrimEnd();
            }
            else if (competicao.Contains("2014"))
            {
                competicao = competicao.Replace("2014", "").TrimEnd();
            }
            for (int i = 2001; i < 2022; i++)
            {
                if (competicao.EndsWith(i.ToString()))
                {
                    competicao = competicao.Replace(i.ToString(), "").TrimEnd();
                    break;
                }
            }
            for (int i = 01; i < 22; i++)
            {
                if (competicao.EndsWith(i.ToString()))
                {
                    competicao = competicao.Replace(i.ToString(), "").TrimEnd();
                    break;
                }
            }

            foreach (KeyValuePair<string, string> item in nomesCampeonatosParaAtt)
            {
                if (competicao.Contains(item.Key))
                {
                    competicao = item.Value;
                }
            }

            if (competicao == "Pernambucano" || competicao == "Cearense" || competicao == "Baiano" || competicao == "Amazonense" || competicao == "Catarinense" ||
                competicao == "Gaúcho" || competicao == "Mineiro" || competicao == "Paulista" || competicao == "Paranaense" || competicao == "Goiano" ||
                competicao == "Sergipano" || competicao == "Piauiense" || competicao == "Maranhense" || competicao == "Brasiliense")
            {
                competicao = $"Campeonato {competicao}";
            }

            jogo.Data = data;
            jogo.TimeCasa = infoPartida[3].InnerText.Replace("´", "'");
            jogo.TimeFora = infoPartida[5].InnerText.Replace("´", "'");
            jogo.Rodada = fase;
            jogo.Campeonato = competicao;
            jogo.NomePaisTimeCasa = nomePaisCasa;
            jogo.NomePaisTimeFora = nomePaisFora;
            jogo.PlacarCasa = placarTimeCasa;
            jogo.PlacarFora = placarTimeFora;
            jogo.IdPartida = idPartida;

            return jogo;
        }

        private void CriarPartidasCampeonatoBrasileiro()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv")
                    .Select(a => a.Split(','))
                    .Select(c => new Dados()
                    {
                        Horario = c[0],
                        Dia = c[1],
                        Data = c[2],
                        TimeCasa = c[3],
                        TimeFora = c[4],
                        Vencedor = c[5],
                        Rodada = c[6],
                        Arena = c[7],
                        PlacarTimeCasa = c[8],
                        PlacarTimeFora = c[9],
                        EstadoTimeCasa = c[10],
                        EstadoTimeFora = c[11],
                        Campeonato = "1",
                        Pais = "1"
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.Data == "Data")
                {
                    continue;
                }
                DateTime dataComHoras;
                char separador;
                if (info.Horario.Contains('h'))
                {
                    separador = 'h';
                }
                else if (info.Horario.Contains(':'))
                {
                    separador = ':';
                }
                else
                {
                    separador = ' ';
                }
                string[] dataSeparada = info.Data.Split('-');
                string[] horaSeparada = info.Horario.Split(separador);

                if (horaSeparada.Length > 0)
                {
                    if (horaSeparada.Length > 1)
                    {
                        if (string.IsNullOrEmpty(horaSeparada[1]))
                        {
                            horaSeparada[1] = "0";
                        }
                        dataComHoras = new DateTime(Convert.ToInt32(dataSeparada[0]), Convert.ToInt32(dataSeparada[1]), Convert.ToInt32(dataSeparada[2]),
                                                    Convert.ToInt32(horaSeparada[0]), Convert.ToInt32(horaSeparada[1]), 0);
                    }
                    else
                    {
                        dataComHoras = new DateTime(Convert.ToInt32(dataSeparada[0]), Convert.ToInt32(dataSeparada[1]), Convert.ToInt32(dataSeparada[2]));
                    }
                }
                else
                {
                    dataComHoras = new DateTime(Convert.ToInt32(dataSeparada[0]), Convert.ToInt32(dataSeparada[1]), Convert.ToInt32(dataSeparada[2]));
                }

                info.Data = dataComHoras.ToString();
                const string modalidade = "1";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                string idEstadio = RetornaIDEstadio(info.Arena);
                string idRodada = RetornaIDRodada(info.Rodada);

                #region Verifica Rodada
                if (string.IsNullOrEmpty(idRodada))
                {
                    string nomeRodada = info.Rodada;
                    info.Rodada = CriarRodada(nomeRodada).ToString();
                    idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), info.Rodada);
                    rodadasMigradas++;
                }
                else
                    info.Rodada = idRodada;
                #endregion

                #region Verifica Estádio
                if (string.IsNullOrEmpty(idEstadio))
                {
                    string nomeEstadio = info.Arena;
                    info.Arena = CriarEstadio(nomeEstadio).ToString();
                    estadiosPorNome.Add(RemoveAccents(nomeEstadio.ToUpper()), info.Arena);
                    estadiosMigrados++;
                }
                else
                    info.Arena = idEstadio;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, info.Pais, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, info.Pais, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartida(info);
                    partidasMigradas++;

                    if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), info.Rodada))
                    {
                        CriarRodadaPartida(idPartida.ToString(), info.Rodada);
                        rodadapartidasMigradas++;
                    }
                }

                AtualizarResultado();
            }
        }

        private void CriarPartidasCopaDoMundo()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv")
                    .Select(a => a.Split(','))
                    .Select(c => new Dados()
                    {
                        Edicao = c[0],
                        Data = c[1],
                        Rodada = c[2],
                        Cidade = c[3],
                        TimeCasa = c[4],
                        PlacarTimeCasa = c[5],
                        PlacarTimeFora = c[6],
                        TimeFora = c[7],
                        Campeonato = "13"
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.Edicao == "Anos")
                {
                    continue;
                }
                const string modalidade = "1";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                string idRodada = RetornaIDRodada(info.Rodada);

                #region Verifica Rodada
                if (string.IsNullOrEmpty(idRodada))
                {
                    string nomeRodada = info.Rodada;
                    info.Rodada = CriarRodada(nomeRodada).ToString();
                    idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), info.Rodada);
                    rodadasMigradas++;
                }
                else
                    info.Rodada = idRodada;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, info.Pais, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, info.Pais, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartidaCopaDoMundo(info);
                    partidasMigradas++;

                    if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), info.Rodada))
                    {
                        CriarRodadaPartida(idPartida.ToString(), info.Rodada);
                        rodadapartidasMigradas++;
                    }
                }

                AtualizarResultado();
            }
        }

        private void CriarPartidasCopaDoMundo2018()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv", Encoding.UTF7)
                    .Select(a => a.Split(';'))
                    .Select(c => new Dados()
                    {
                        Data = c[0],
                        Rodada = c[1],
                        Arena = c[2],
                        Cidade = c[3],
                        TimeCasa = c[4],
                        PlacarTimeCasa = c[5],
                        PlacarTimeFora = c[6],
                        TimeFora = c[7],
                        Campeonato = "13"
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.Data == "Data")
                {
                    continue;
                }
                const string modalidade = "1";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                string idRodada = RetornaIDRodada(info.Rodada);
                string idPaisCasa = paisesPorNome[RemoveAccents(info.TimeCasa.ToUpper())];
                string idPaisFora = paisesPorNome[RemoveAccents(info.TimeFora.ToUpper())];

                #region Verifica Rodada
                if (string.IsNullOrEmpty(idRodada))
                {
                    string nomeRodada = info.Rodada;
                    info.Rodada = CriarRodada(nomeRodada).ToString();
                    idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), info.Rodada);
                    rodadasMigradas++;
                }
                else
                    info.Rodada = idRodada;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, idPaisCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, idPaisFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartidaCopaDoMundo(info);
                    partidasMigradas++;

                    if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), info.Rodada))
                    {
                        CriarRodadaPartida(idPartida.ToString(), info.Rodada);
                        rodadapartidasMigradas++;
                    }
                }

                AtualizarResultado();
            }
        }

        private void CriarPartidasNFL()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv", Encoding.UTF7)
                    .Select(a => a.Split(';'))
                    .Select(c => new Dados()
                    {
                        Data = c[0],
                        Edicao = c[1],
                        Rodada = c[2],
                        TimeCasa = c[3],
                        PlacarTimeCasa = c[4],
                        PlacarTimeFora = c[5],
                        TimeFora = c[6],
                        Arena = c[7],
                        Campeonato = "32"
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.Data == "Data")
                {
                    continue;
                }
                const string modalidade = "3";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                string idEstadio = RetornaIDEstadio(info.Arena);
                string idRodada = RetornaIDRodada(info.Rodada);
                string idPaisCasa = "2";
                string idPaisFora = "2";

                info.Data = DateTime.Parse(info.Data, new CultureInfo("en-US")).ToString();

                #region Verifica Rodada
                if (string.IsNullOrEmpty(idRodada))
                {
                    string nomeRodada = info.Rodada;
                    info.Rodada = CriarRodada(nomeRodada).ToString();
                    idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), info.Rodada);
                    rodadasMigradas++;
                }
                else
                    info.Rodada = idRodada;
                #endregion

                #region Verifica Estádio
                if (string.IsNullOrEmpty(idEstadio))
                {
                    string nomeEstadio = info.Arena;
                    info.Arena = CriarEstadio(nomeEstadio).ToString();
                    estadiosPorNome.Add(RemoveAccents(nomeEstadio.ToUpper()), info.Arena);
                    estadiosMigrados++;
                }
                else
                    info.Arena = idEstadio;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, idPaisCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, idPaisFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartida(info);
                    partidasMigradas++;

                    if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), info.Rodada))
                    {
                        CriarRodadaPartida(idPartida.ToString(), info.Rodada);
                        rodadapartidasMigradas++;
                    }
                }
                else
                {
                    partidasJaExistem++;
                }

                AtualizarResultado();
            }
        }

        private void CriarPartidasNBA()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv", Encoding.UTF7)
                    .Select(a => a.Split(';'))
                    .Select(c => new Dados()
                    {
                        TimeCasa = c[0],
                        PlacarTimeCasa = c[1],
                        TimeFora = c[2],
                        PlacarTimeFora = c[3],
                        Data = c[4],
                        Rodada = c[5],
                        Campeonato = "30"
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.TimeCasa == "TimeCasa")
                {
                    continue;
                }
                const string modalidade = "2";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                if (info.Rodada == "False")
                    info.Rodada = "Fase de Grupos";
                else
                    info.Rodada = "Playoff";
                string idRodada = RetornaIDRodada(info.Rodada);
                string idPaisCasa = "2";
                string idPaisFora = "2";

                info.Data = DateTime.Parse(info.Data, new CultureInfo("en-US")).ToString();

                #region Verifica Rodada
                if (string.IsNullOrEmpty(idRodada))
                {
                    string nomeRodada = info.Rodada;
                    info.Rodada = CriarRodada(nomeRodada).ToString();
                    idRodadaPorDescricao.Add(RemoveAccents(nomeRodada).ToUpper(), info.Rodada);
                    rodadasMigradas++;
                }
                else
                    info.Rodada = idRodada;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, idPaisCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, idPaisFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartidaNBA(info);
                    partidasMigradas++;

                    if (!VerificaSeRodadaPartidaExiste(idPartida.ToString(), info.Rodada))
                    {
                        CriarRodadaPartida(idPartida.ToString(), info.Rodada);
                        rodadapartidasMigradas++;
                    }
                }

                AtualizarResultado();
            }
        }

        private void CriarPartidasAllStarNBA()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv", Encoding.UTF7)
                    .Select(a => a.Split(';'))
                    .Select(c => new Dados()
                    {
                        Data = c[0],
                        Arena = c[1],
                        TimeCasa = c[2],
                        PlacarTimeCasa = c[3],
                        TimeFora = c[4],
                        PlacarTimeFora = c[5],
                        Campeonato = "30"
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.Data == "Data")
                {
                    continue;
                }

                string nomeEstadio = info.Arena.Split(',')[0].TrimEnd();
                string nomeCidade = info.Arena.Split(',')[1].TrimStart().TrimEnd();
                string siglaEstado = info.Arena.Split(',')[2].TrimStart();
                string idEstadio = RetornaIDEstadio(nomeEstadio);
                string idEstado = RetornaIDEstado(siglaEstado);
                string idCidade = RetornaIDCidade(nomeCidade, siglaEstado);
                const string modalidade = "2";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                string idPaisCasa = "2";
                string idPaisFora = "2";

                #region Verifica Estádio
                if (string.IsNullOrEmpty(idEstadio))
                {
                    info.Arena = CriarEstadio(nomeEstadio).ToString();
                    estadiosPorNome.Add(RemoveAccents(nomeEstadio.ToUpper()), info.Arena);
                    estadiosMigrados++;
                }
                else
                    info.Arena = idEstadio;
                #endregion

                #region Verifica Estado
                if (string.IsNullOrEmpty(idEstado))
                {
                    info.EstadoTimeCasa = CriarEstado(siglaEstado, idPaisCasa).ToString();
                    estadosPorNomePais.Add(RemoveAccents(RetornarEstadoDosEUA(siglaEstado).ToUpper()) + ";ESTADOS UNIDOS", info.EstadoTimeCasa);
                    estadosMigrados++;
                }
                else
                    info.EstadoTimeCasa = idEstado;
                #endregion

                #region Verifica Cidade
                if (string.IsNullOrEmpty(idCidade))
                {
                    info.Cidade = CriarCidades(nomeCidade, Convert.ToInt32(info.EstadoTimeCasa), Convert.ToInt32(idPaisCasa)).ToString();
                    cidadesPorNomePaisEstado.Add(RemoveAccents(nomeCidade.ToUpper()) + ";ESTADOS UNIDOS;" + RemoveAccents(RetornarEstadoDosEUA(siglaEstado).ToUpper()), info.Cidade);
                    cidadesMigradas++;
                }
                else
                    info.Cidade = idCidade;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, idPaisCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, idPaisFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartidaNBAAllStar(info);
                    partidasMigradas++;
                }

                AtualizarResultado();
            }
        }

        private void CriarPartidasSelecoes()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv")
                    .Select(a => a.Split(','))
                    .Select(c => new Dados()
                    {
                        Data = c[0],
                        TimeCasa = c[1],
                        TimeFora = c[2],
                        PlacarTimeCasa = c[3],
                        PlacarTimeFora = c[4],
                        Campeonato = c[5],
                        Cidade = c[6],
                        Pais = c[7]
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.Data == "Data")
                {
                    continue;
                }
                info.Data = DateTime.Parse(info.Data, new CultureInfo("en-US")).ToString();
                string nomeCidade = info.Cidade;
                string idCidade = RetornaIDCidade(nomeCidade, "", info.Pais);
                const string modalidade = "1";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                string idCampeonato = RetornaIDCampeonato(info.Campeonato);
                string idPaisTimeCasa = RetornaIDPais(info.TimeCasa);
                string idPaisTimeFora = RetornaIDPais(info.TimeFora);
                string idPaisJogo = RetornaIDPais(info.Pais);

                #region Verifica Pais-Casa
                if (string.IsNullOrEmpty(idPaisTimeCasa))
                {
                    idPaisTimeCasa = CriarPaises(info.TimeCasa).ToString();
                    paisesPorNome.Add(RemoveAccents(info.TimeCasa.ToUpper()), idPaisTimeCasa);
                    paisesMigrados++;
                }
                #endregion

                #region Verifica Pais-Fora
                if (string.IsNullOrEmpty(idPaisTimeFora))
                {
                    idPaisTimeFora = CriarPaises(info.TimeFora).ToString();
                    paisesPorNome.Add(RemoveAccents(info.TimeFora.ToUpper()), idPaisTimeFora);
                    paisesMigrados++;
                }
                #endregion

                #region Verifica Pais-Jogo
                if (string.IsNullOrEmpty(idPaisJogo))
                {
                    if (!paisesPorNome.ContainsKey(RemoveAccents(info.Pais.ToUpper())))
                    {
                        idPaisJogo = CriarPaises(info.Pais).ToString();
                        paisesPorNome.Add(RemoveAccents(info.Pais.ToUpper()), idPaisJogo);
                        paisesMigrados++;
                    }
                    else
                    {
                        idPaisJogo = paisesPorNome[RemoveAccents(info.Pais.ToUpper())];
                    }
                }
                #endregion

                #region Verifica Cidade
                if (string.IsNullOrEmpty(idCidade))
                {
                    info.Cidade = CriarCidades(nomeCidade, -1, Convert.ToInt32(idPaisJogo)).ToString();
                    cidadesPorNomePaisEstado.Add(RemoveAccents(nomeCidade.ToUpper()) + ";" + RemoveAccents(info.Pais.ToUpper()) + ";", info.Cidade);
                    cidadesMigradas++;
                }
                else
                    info.Cidade = idCidade;
                #endregion

                #region Verifica Campeonato
                if (string.IsNullOrEmpty(idCampeonato))
                {
                    string nomeCampeonato = info.Campeonato;
                    info.Campeonato = CriarCampeonato(nomeCampeonato).ToString();
                    campeonatosPorNome.Add(RemoveAccents(nomeCampeonato).ToUpper(), info.Campeonato);
                    campeonatosMigrados++;
                }
                else
                    info.Campeonato = idCampeonato;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, idPaisTimeCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, idPaisTimeFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartidaDeSelecoes(info);
                    partidasMigradas++;
                }

                AtualizarResultado();
            }
        }

        private void CriarPartidasDeClubesMundo()
        {
            var list = File.ReadAllLines(Diretorio + cbPlanilha.SelectedItem + ".csv", Encoding.UTF7)
                    .Select(a => a.Split(';'))
                    .Select(c => new Dados()
                    {
                        TimeCasa = c[0],
                        TimeFora = c[1],
                        Data = c[2],
                        PlacarTimeCasa = c[3],
                        PlacarTimeFora = c[4],
                        Campeonato = c[5],
                        PaisSede = c[6],
                        Pais = c[7]
                    }).ToList();

            foreach (Dados info in list)
            {
                if (info.TimeCasa == "TimeCasa")
                {
                    continue;
                }
                const string modalidade = "1";
                string idTimeCasa = RetornaIDTime(info.TimeCasa, modalidade);
                string idTimeFora = RetornaIDTime(info.TimeFora, modalidade);
                string idCampeonato = RetornaIDCampeonato(info.Campeonato);
                string idPaisTimeCasa = RetornaIDPais(info.PaisSede);
                string idPaisTimeFora = RetornaIDPais(info.Pais);

                #region Verifica Pais-Casa
                if (string.IsNullOrEmpty(idPaisTimeCasa))
                {
                    idPaisTimeCasa = CriarPaises(info.PaisSede).ToString();
                    paisesPorNome.Add(RemoveAccents(info.PaisSede.ToUpper()), idPaisTimeCasa);
                    paisesMigrados++;
                }
                #endregion

                #region Verifica Pais-Fora
                if (string.IsNullOrEmpty(idPaisTimeFora))
                {
                    if (!paisesPorNome.ContainsKey(RemoveAccents(info.Pais.ToUpper())))
                    {
                        idPaisTimeFora = CriarPaises(info.Pais).ToString();
                        paisesPorNome.Add(RemoveAccents(info.Pais.ToUpper()), idPaisTimeFora);
                        paisesMigrados++;
                    }
                    else
                    {
                        idPaisTimeFora = paisesPorNome[RemoveAccents(info.Pais.ToUpper())];
                    }
                }
                #endregion

                #region Verifica Campeonato
                if (string.IsNullOrEmpty(idCampeonato))
                {
                    string nomeCampeonato = info.Campeonato;
                    info.Campeonato = CriarCampeonato(nomeCampeonato).ToString();
                    campeonatosPorNome.Add(RemoveAccents(nomeCampeonato).ToUpper(), info.Campeonato);
                    campeonatosMigrados++;
                }
                else
                    info.Campeonato = idCampeonato;
                #endregion

                #region Verifica Time da Casa
                if (string.IsNullOrEmpty(idTimeCasa))
                {
                    info.TimeCasa = CriarTime(info.TimeCasa, idPaisTimeCasa, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeCasa = idTimeCasa;
                #endregion

                #region Verifica Time da Fora
                if (string.IsNullOrEmpty(idTimeFora))
                {
                    info.TimeFora = CriarTime(info.TimeFora, idPaisTimeFora, modalidade).ToString();
                    timesMigrados++;
                }
                else
                    info.TimeFora = idTimeFora;
                #endregion

                if (!VerificaSePartidaExiste(info.TimeCasa, info.TimeFora, info.Data))
                {
                    int idPartida = CriarPartidaDeClubesMundo(info);
                    partidasMigradas++;
                }

                AtualizarResultado();
            }
        }

        private void CriarEstatisticas()
        {
            List<InfoEstatisticas> listaEstatisticas = new List<InfoEstatisticas>();
            foreach (var item in listaPartidasEmMemoria.GroupBy(p => new { p.IdTimeCasa, p.IdTimeFora }).Select(s => new { s.Key.IdTimeCasa, s.Key.IdTimeFora }))
            {
                if (listaEstatisticas.Exists(p => p.IdTimeFora == Convert.ToInt32(item.IdTimeCasa) && p.IdTimeCasa == Convert.ToInt32(item.IdTimeFora)))
                {
                    InfoEstatisticas info = listaEstatisticas.Find(p => p.IdTimeFora == Convert.ToInt32(item.IdTimeCasa) && p.IdTimeCasa == Convert.ToInt32(item.IdTimeFora));
                    info.Total += listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora)).Count();
                    info.Vitorias += listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora) && (Convert.ToInt32(p.PlacarTimeCasa) < Convert.ToInt32(p.PlacarTimeFora))).Count();
                    info.Derrotas += listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora) && (Convert.ToInt32(p.PlacarTimeCasa) > Convert.ToInt32(p.PlacarTimeFora))).Count();
                    info.Empates += listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora) && (Convert.ToInt32(p.PlacarTimeCasa) == Convert.ToInt32(p.PlacarTimeFora))).Count();
                }
                else
                {
                    InfoEstatisticas info = new InfoEstatisticas();
                    info.Total = listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora)).Count();
                    info.Vitorias = listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora) && (Convert.ToInt32(p.PlacarTimeCasa) > Convert.ToInt32(p.PlacarTimeFora))).Count();
                    info.Derrotas = listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora) && (Convert.ToInt32(p.PlacarTimeCasa) < Convert.ToInt32(p.PlacarTimeFora))).Count();
                    info.Empates = listaPartidasEmMemoria.Where(p => (p.IdTimeCasa == item.IdTimeCasa) && (p.IdTimeFora == item.IdTimeFora) && (Convert.ToInt32(p.PlacarTimeCasa) == Convert.ToInt32(p.PlacarTimeFora))).Count();
                    info.IdTimeCasa = Convert.ToInt32(item.IdTimeCasa);
                    info.IdTimeFora = Convert.ToInt32(item.IdTimeFora);
                    listaEstatisticas.Add(info);
                }
            }

            foreach (InfoEstatisticas info in listaEstatisticas)
            {
                if (!VerificaSeEstatisticaExiste(info.IdTimeCasa.ToString(), info.IdTimeFora.ToString()))
                {
                    int idEstat = CriarEstatisticasNoBanco(info);
                    estatMigradas++;
                }

                AtualizarResultado();
            }
        }
        #endregion

        public class Jogos
        {
            public DateTime Data { get; set; }
            public string TimeCasa { get; set; }
            public string TimeFora { get; set; }
            public int PlacarCasa { get; set; }
            public int PlacarFora { get; set; }
            public string Rodada { get; set; }
            public string Campeonato { get; set; }
            public string IdPartida { get; set; }
            public string NomePaisTimeCasa { get; set; }
            public string NomePaisTimeFora { get; set; }
        }

        public class InfoEstatisticas
        {
            public int IdTimeCasa { get; set; }
            public int IdTimeFora { get; set; }
            public int Total { get; set; }
            public int Vitorias { get; set; }
            public int Empates { get; set; }
            public int Derrotas { get; set; }
        }

        public class InfoClassificacao
        {
            public int ID { get; set; }
            public int IdUsuario { get; set; }
            public int IdBolao { get; set; }
            public float Total { get; set; }
            public int PlacarCheio { get; set; }
            public int PlacarVencedor { get; set; }
            public int PlacarPerdedor { get; set; }
            public int AcertouVencedor { get; set; }
            public int Variacao { get; set; }
            public int Posicao { get; set; }
            public int PosicaoAnterior { get; set; }
        }

        public class Dados
        {
            public string Edicao { get; set; }
            public string Posicao { get; set; }
            public string TimeCasa { get; set; }
            public string TimeFora { get; set; }
            public string Pontos { get; set; }
            public string Vitoria { get; set; }
            public string Empate { get; set; }
            public string Derrota { get; set; }
            public string GolsFeitos { get; set; }
            public string GolsContra { get; set; }
            public string SaldoGols { get; set; }
            public string Aproveitamento { get; set; }
            public string PaisSede { get; set; }
            public string Vencedor { get; set; }
            public string Vice { get; set; }
            public string Terceiro { get; set; }
            public string Quarto { get; set; }
            public string QtdSelecoes { get; set; }
            public string QtdPartidas { get; set; }
            public string Horario { get; set; }
            public string Dia { get; set; }
            public string Data { get; set; }
            public string Rodada { get; set; }
            public string Arena { get; set; }
            public string PlacarTimeCasa { get; set; }
            public string PlacarTimeFora { get; set; }
            public string EstadoTimeCasa { get; set; }
            public string EstadoTimeFora { get; set; }
            public string Cidade { get; set; }
            public string Campeonato { get; set; }
            public string Pais { get; set; }
        }

        public class InfoJogosAtrasadosNBA
        {
            public string URL { get; set; }
            public List<int> PartidasPorDia { get; set; }
            public List<string> Datas { get; set; }
        }

        #region Classes em Memoria
        public class PaisesEmMemoria
        {
            public string ID { get; set; }
            public string Nome { get; set; }
        }
        public class EstadosEmMemoria
        {
            public string ID { get; set; }
            public string Nome { get; set; }
            public string IdPais { get; set; }
            public string NomePais { get; set; }
        }
        public class CidadesEmMemoria
        {
            public string ID { get; set; }
            public string Nome { get; set; }
            public string IdEstado { get; set; }
            public string IdPais { get; set; }
            public string NomeEstado { get; set; }
            public string NomePais { get; set; }
        }
        public class TimesEmMemoria
        {
            public string ID { get; set; }
            public string Nome { get; set; }
            public string IdCidade { get; set; }
            public string IdEstado { get; set; }
            public string IdPais { get; set; }
            public string NomeCidade { get; set; }
            public string NomeEstado { get; set; }
            public string NomePais { get; set; }
            public string Modalidade { get; set; }
        }
        public class ClassificacaoEmMemoria
        {
            public string ID { get; set; }
            public string IdUsuario { get; set; }
            public string IdBolao { get; set; }
            public string Total { get; set; }
            public string PlacarCheio { get; set; }
            public string PlacarVencedor { get; set; }
            public string PlacarPerdedor { get; set; }
            public string AcertouVencedor { get; set; }
            public string Variacao { get; set; }
            public string Posicao { get; set; }
            public string PosicaoAnterior { get; set; }
        }
        public class PalpitesEmMemoria
        {
            public string ID { get; set; }
            public string IdUsuario { get; set; }
            public string IdBolao { get; set; }
            public string IdTimeCasa { get; set; }
            public string IdTimeFora { get; set; }
            public string PalpiteTimeCasa { get; set; }
            public string PalpiteTimeFora { get; set; }
            public string IdPartida { get; set; }
            public string Contabilizado { get; set; }
        }
        public class EstatisticasEmMemoria
        {
            public string ID { get; set; }
            public string NomeTimeA { get; set; }
            public string NomeTimeB { get; set; }
            public string IdTimeA { get; set; }
            public string IdTimeB { get; set; }
            public string VitoriasTimeA { get; set; }
            public string VitoriasTimeB { get; set; }
            public string Empates { get; set; }
        }
        public class CampeonatosEmMemoria
        {
            public string ID { get; set; }
            public string Nome { get; set; }
        }
        public class PartidasEmMemoria
        {
            public string ID { get; set; }
            public string IdTimeCasa { get; set; }
            public string IdTimeFora { get; set; }
            public string IdCampeonato { get; set; }
            public string IdEstadio { get; set; }
            public string Data { get; set; }
            public string PlacarTimeCasa { get; set; }
            public string PlacarTimeFora { get; set; }
            public string IdConversao { get; set; }
        }
        public class RegrasEmMemoria
        {
            public string ID { get; set; }
            public string Pontuacao1 { get; set; }
            public string Pontuacao2 { get; set; }
            public string Pontuacao3 { get; set; }
        }
        public class RegraBolaoEmMemoria
        {
            public string ID { get; set; }
            public string IdBolao { get; set; }
            public string IdRegra { get; set; }
        }
        public class RodadaEmMemoria
        {
            public string ID { get; set; }
            public string Descricao { get; set; }
        }
        public class RodadaPartidaEmMemoria
        {
            public string ID { get; set; }
            public string IdPartida { get; set; }
            public string IdRodada { get; set; }
        }
        public class EstadioEmMemoria
        {
            public string ID { get; set; }
            public string Nome { get; set; }
        }
        #endregion
    }
}