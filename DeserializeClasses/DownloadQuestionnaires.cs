using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExempleAnketaKYCService.DeserializeClasses
{
    /// <summary>
    /// Список id анкет готовых для выгрузки
    /// </summary>
    [XmlRoot(Namespace = "", ElementName = "questionnaires", IsNullable = true)]
    public class DownloadQuestionnaires
    {
        private Root _NEW;
        private Root _RETURN;

        /// <summary>
        /// id новых анкет готовых к выгрузке
        /// </summary>
        [XmlElement("NEW")]
        public Root NEW
        {
            get { return this._NEW; }
            set { this._NEW = value; }
        }

        /// <summary>
        /// id анкет для обновления
        /// </summary>
        [XmlElement("RETURN")]
        public Root RETURN
        {
            get { return this._RETURN; }
            set { this._RETURN = value; }
        }
    }

    public partial class Root
    {
        private List<string> _IDs;

        /// <summary>
        /// id анкет
        /// </summary>
        [XmlElement("id")]
        public List<string> IDs
        {
            get { return this._IDs; }
            set { this._IDs = value; }
        }
    }

}