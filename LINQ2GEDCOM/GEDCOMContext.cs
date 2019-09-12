using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQ2GEDCOM.Entities;

namespace LINQ2GEDCOM
{
    /// <summary>
    /// The context which represents the GEDCOM file.  Within this context are the entity
    /// collections which represent the data within the file.
    /// </summary>
    public class GEDCOMContext
    {
        private string _file;
        private string _folder;

        /// <summary>
        /// The file used as the source of GEDCOM data.
        /// </summary>
        public string GEDCOMFile { get { return _file; } }

        /// <summary>
        /// The folder which contains files referenced as objects in the GEDCOM data.
        /// </summary>
        public string GEDCOMObjectFolder { get { return _folder; } }

        /// <summary>
        /// The list of HEAD records.
        /// </summary>
        public IList<Header> Headers { get; internal set; }

        /// <summary>
        /// The list of INDI records.
        /// </summary>
        public IList<Individual> Individuals { get; internal set; }

        /// <summary>
        /// The list of FAM records.
        /// </summary>
        public IList<Family> Families { get; internal set; }

        /// <summary>
        /// The list of SOUR records.
        /// </summary>
        public IList<Source> Sources { get; internal set; }

        /// <summary>
        /// The list of OBJE records.
        /// </summary>
        public IList<Entities.Object> Objects { get; internal set; }

        /// <summary>
        /// The list of NOTE records.
        /// </summary>
        public IList<Note> Notes { get; internal set; }

        /// <summary>
        /// The list of LABL records.
        /// </summary>
        public IList<Label> Labels { get; internal set; }

        private GEDCOMContext() { }

        /// <summary>
        /// Initialize the data context with a source of data.
        /// </summary>
        /// <param name="file">The file which contains the GEDCOM data.</param>
        public GEDCOMContext(string file, string objectFolder = null)
        {
            _file = file;
            _folder = objectFolder;

            Headers = new List<Header>();
            Individuals = new List<Individual>();
            Families = new List<Family>();
            Sources = new List<Source>();
            Objects = new List<Entities.Object>();
            Notes = new List<Note>();
            Labels = new List<Label>();

            GEDCOMDataReader.LoadDataIntoContext(this);
        }

        /// <summary>
        /// Writes the current state of the data back out to a specified file,
        /// or the current file, replacing all existing contents.
        /// </summary>
        /// <param name="file">The target GEDCOM file.</param>
        public void SubmitChanges(string file = null)
        {
            GEDCOMDataWriter.WriteToFile(this, file ?? _file);
        }
    }
}
