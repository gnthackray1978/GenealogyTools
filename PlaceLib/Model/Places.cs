using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaceLib.Model
{
    public partial class Places
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("placeid")]
        public int Placeid { get; set; }
        [Column("place15cd")]
        [StringLength(50)]
        public string Place15cd { get; set; }
        [Column("placesort")]
        [StringLength(50)]
        public string Placesort { get; set; }
        [Column("place15nm")]
        [StringLength(50)]
        public string Place15nm { get; set; }
        [Column("splitind")]
        [StringLength(50)]
        public string Splitind { get; set; }
        [Column("popcnt")]
        [StringLength(50)]
        public string Popcnt { get; set; }
        [Column("descnm")]
        [StringLength(50)]
        public string Descnm { get; set; }
        [Column("ctyhistnm")]
        [StringLength(50)]
        public string Ctyhistnm { get; set; }
        [Column("ctyltnm")]
        [StringLength(50)]
        public string Ctyltnm { get; set; }
        [Column("ctry15nm")]
        [StringLength(50)]
        public string Ctry15nm { get; set; }
        [Column("cty15cd")]
        [StringLength(50)]
        public string Cty15cd { get; set; }
        [Column("cty15nm")]
        [StringLength(50)]
        public string Cty15nm { get; set; }
        [Column("lad15cd")]
        [StringLength(50)]
        public string Lad15cd { get; set; }
        [Column("lad15nm")]
        [StringLength(50)]
        public string Lad15nm { get; set; }
        [Column("laddescnm")]
        [StringLength(50)]
        public string Laddescnm { get; set; }
        [Column("wd15cd")]
        [StringLength(50)]
        public string Wd15cd { get; set; }
        [Column("par15cd")]
        [StringLength(50)]
        public string Par15cd { get; set; }
        [Column("hlth12cd")]
        [StringLength(50)]
        public string Hlth12cd { get; set; }
        [Column("hlth12nm")]
        [StringLength(50)]
        public string Hlth12nm { get; set; }
        [Column("regd15cd")]
        [StringLength(50)]
        public string Regd15cd { get; set; }
        [Column("regd15nm")]
        [StringLength(50)]
        public string Regd15nm { get; set; }
        [Column("rgn15cd")]
        [StringLength(50)]
        public string Rgn15cd { get; set; }
        [Column("rgn15nm")]
        [StringLength(50)]
        public string Rgn15nm { get; set; }
        [Column("npark15cd")]
        [StringLength(50)]
        public string Npark15cd { get; set; }
        [Column("npark15nm")]
        [StringLength(50)]
        public string Npark15nm { get; set; }
        [Column("bua11cd")]
        [StringLength(50)]
        public string Bua11cd { get; set; }
        [Column("pcon15cd")]
        [StringLength(50)]
        public string Pcon15cd { get; set; }
        [Column("pcon15nm")]
        [StringLength(50)]
        public string Pcon15nm { get; set; }
        [Column("eer15cd")]
        [StringLength(50)]
        public string Eer15cd { get; set; }
        [Column("eer15nm")]
        [StringLength(50)]
        public string Eer15nm { get; set; }
        [Column("pfa15cd")]
        [StringLength(50)]
        public string Pfa15cd { get; set; }
        [Column("pfa15nm")]
        [StringLength(50)]
        public string Pfa15nm { get; set; }
        [Column("gridgb1m")]
        [StringLength(50)]
        public string Gridgb1m { get; set; }
        [Column("gridgb1e")]
        [StringLength(50)]
        public string Gridgb1e { get; set; }
        [Column("gridgb1n")]
        [StringLength(50)]
        public string Gridgb1n { get; set; }
        [Column("grid1km")]
        [StringLength(50)]
        public string Grid1km { get; set; }
        [Column("lat")]
        [StringLength(50)]
        public string Lat { get; set; }
        [Column("long")]
        [StringLength(50)]
        public string Long { get; set; }
        [Column("FID")]
        [StringLength(50)]
        public string Fid { get; set; }
    }
}
