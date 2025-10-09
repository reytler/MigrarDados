using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrarDados.Models;

public class Caged
{
    public int Id { get; set; }
    [StringLength(10)]
    public string Secao {  get; set; }
    [StringLength(20)]
    public string CdMunicipio { get; set; }
    [StringLength(255)]
    public string Municipio { get; set; }

    [StringLength(2)]
    public string Uf { get; set; }

    [StringLength(50)]
    public string FaixaEmpregados { get; set; }
    
    public DateTime Competencia { get; set; }
    public long Fluxo { get; set; }
}
