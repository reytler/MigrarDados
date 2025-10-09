using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrarDados.Models;

public class CagedSQLite
{
    [Column("secao")]
    public string Secao { get; set; } = string.Empty;
    [Column("cd_municipio")]
    public string CdMunicipio { get; set; } = string.Empty;
    [Column("municipio")]
    public string Municipio { get; set; } = string.Empty;
    [Column("uf")]
    public string Uf { get; set; } = string.Empty;
    [Column("faixa_empregados")]
    public string FaixaEmpregados { get; set; } = string.Empty;
    [Column("competencia")]
    public DateTime Competencia { get; set; }
    [Column("fluxo")]
    public long Fluxo { get; set; }
}
