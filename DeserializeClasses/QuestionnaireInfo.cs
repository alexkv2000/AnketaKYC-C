using System.Collections.Generic;
using System.Xml.Serialization;

using System;
using System.Linq;

namespace ExempleAnketaKYCService.DeserializeClasses
{
    /// <summary>
    /// Информация по анкете
    /// </summary>
    [XmlRoot(Namespace = "", ElementName = "questionnaire", IsNullable = true)]
    public class QuestionnaireInfo
    {
        private ContragentInfo _ContragentInfo;
        private TagInfo<GazConpanyInfo> _GGAZLEInfo;
        private string _CreateDate_string;

        [XmlIgnore]
        public string TextXml { get; set; }

        /// <summary>
        /// Дата документа(строка)
        /// </summary>
        [XmlElement("CREATE_DATE")]
        public string CreateDate_string
        {
            get { return this._CreateDate_string; }
            set { this._CreateDate_string = value; }
        }

        /// <summary>
        /// Дата создания
        /// </summary>
        [XmlIgnore]
        public DateTime? CreateDate
        {
            get { return string.IsNullOrWhiteSpace(_CreateDate_string) ? (DateTime?)null : DateTime.Parse(_CreateDate_string); }
        }

        /// <summary>
        /// Информация по контрагенту
        /// </summary>
        [XmlElement("P1_PROP_2_1")]
        public ContragentInfo ContragentInfo
        {
            get { return this._ContragentInfo; }
            set { this._ContragentInfo = value; }
        }

        /// <summary>
        /// Информация по ЮЛ Группы ГАЗ
        /// </summary>
        [XmlElement("COMPANY")]
        public TagInfo<GazConpanyInfo> GGAZLEInfo
        {
            get { return this._GGAZLEInfo; }
            set { this._GGAZLEInfo = value; }
        }

        /// <summary>
        /// Информация по контрагенту
        /// </summary>
        [XmlElement("KUR_EMAIL")]
        public TagInfo<string> KurEmail { get; set; }

        /// <summary>
        /// Должная осмотрительность
        /// </summary>
        [XmlElement("DueDiligence")]
        public TagInfo<string> DueDiligence { get; set; }

        /// <summary>
        /// Санкционный риск
        /// </summary>
        [XmlElement("SanctionRisk")]
        public TagInfo<string> SanctionRisk { get; set; }

        /// <summary>
        /// Тип договора
        /// </summary>
        [XmlElement("CONTRACT_TYPE")]
        public TagInfo<string> TypeContract { get; set; }

        /// <summary>
        /// Маркер неблагонадёжности
        /// </summary>
        [XmlElement("marker")]
        public TagInfo<string> Unreliability { get; set; }

        /// <summary>
        /// Маркер Должная осмотрительность\неблагонадёжности
        /// </summary>
        [XmlElement("route_marker")]
        public TagInfo<string> RouteMarker { get; set; }

        private TagInfo<AnketFILE> _FILEs;
        /// <summary>
        /// Информация о файле анкеты
        /// </summary>
        [XmlElement("PROP_11")]
        public TagInfo<AnketFILE> AnketFiles
        {
            get { return this._FILEs; }
            set { this._FILEs = value; }
        }

        private TagInfo<AnketFILE> _FILEXLS;
        /// <summary>
        /// Информация о файле анкеты
        /// </summary>
        [XmlElement("FILE_XLS")]
        public TagInfo<AnketFILE> AnketFilesXLS
        {
            get { return this._FILEXLS; }
            set { this._FILEXLS = value; }
        }


        private TagInfo<string> _hiddenserviceStatus;
        /// <summary>
        /// Скрытый статус в сервисе(на сайте)
        /// </summary>
        [XmlElement("STATUS")]
        public TagInfo<string> ServiceStatusInfo {
            get { return this._hiddenserviceStatus; }
            set { this._hiddenserviceStatus = value; }
        }

        

        [XmlIgnore]
        public AnketServiceStatus ServiceStatus
        {
            get {
                return ConvertAnketServiceStatus(this._hiddenserviceStatus.Values?.FirstOrDefault()); }
        }

        internal AnketServiceStatus ConvertAnketServiceStatus(string status)
        {
            Console.WriteLine($"status text: {(string.IsNullOrWhiteSpace(status)? "null": status)}");
            if (string.IsNullOrWhiteSpace(status)) return AnketServiceStatus.notset;
            switch (status)
            {
                case "Редактирование":
                    return AnketServiceStatus.edit;
                case "На доработке":
                    return AnketServiceStatus.return_edit;
                case "Отменена":
                    return AnketServiceStatus.cancel;
                case "Отправлена":
                    return AnketServiceStatus.sent;
                default:
                    return AnketServiceStatus.notset;
            }
        }

    }

    

    public enum AnketServiceStatus
    {
        /// <summary>
        /// Редактирование
        /// </summary>
        edit,
        /// <summary>
        /// На доработке
        /// </summary>
        return_edit,
        /// <summary>
        /// Отменена
        /// </summary>
        cancel,
        /// <summary>
        /// Отправлена
        /// </summary>
        sent,
        /// <summary>
        /// Не задан
        /// </summary>
        notset

    }


/// <summary>
/// Информация по контрагенту
/// </summary>
//[XmlRootAttribute( ElementName = "P1_PROP_2_1", IsNullable = true)]
public partial class ContragentInfo
    {
        private Values<MoreInfo> _ContragentInfoValues;

        [XmlElement("values")]
        public Values<MoreInfo> ContragentInfoValues
        {
            get { return this._ContragentInfoValues; }
            set { this._ContragentInfoValues = value; }
        }

        [XmlIgnore]
        public List<MoreInfo> Values
        {
            get
            {
                if (this._ContragentInfoValues.values == null)
                {
                    //Console.WriteLine($"tagValues is null: {tagValues == null}");
                    return null;
                }
                else return this._ContragentInfoValues.values;
            }
        }
    }


    public partial class Values<T>
    {
        private List<T> _value;

        /// <summary>
        /// значения
        /// </summary>
        [XmlElement("value")]
        public List<T> values
        {
            get { return this._value; }
            set { this._value = value; }
        }
    }

    /// <summary>
    /// Значения в теге(общее)
    /// </summary>
    public class TagInfo<T>
    {
        private Values<T> _tagValues;

        /// <summary>
        /// Описание
        /// </summary>
        [XmlElement("title")]
        public string title { get; set; }



        /// <summary>
        /// Значения
        /// </summary>
        [XmlElement("values")]
        public Values<T> tagValues
        {
            get { return this._tagValues; }
            set { this._tagValues = value; }
        }

        [XmlIgnore]
        public List<T> Values
        {
            get
            {
                if (this._tagValues == null)
                {
                    return null;
                }
                else return this._tagValues.values;
            }
        }

    }

    public class MoreInfo
    {
        private AnketFILE _FILE;

        /// <summary>
        /// ИНН
        /// </summary>
        [XmlElement("INN")]
        public TagInfo<string> INN { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [XmlElement("NAME")]
        public TagInfo<string> NAME { get; set; }

        /// <summary>
        /// Информация о файле анкеты
        /// </summary>
        [XmlElement("FILE")]
        public AnketFILE FILE
        {
            get { return this._FILE; }
            set { this._FILE = value; }
        }
    }

    /*public class GazConpanyInfo
    {
        private Values<GazConpanyMoreInfo> _GazConpanyMoreInfo;

        /// <summary>
        /// Описание
        /// </summary>
        [XmlElement("title")]
        public string title { get; set; }

        /// <summary>
        /// Данные ЮЛ Группы ГАЗ
        /// </summary>
        [XmlElement("values")]
        public Values<GazConpanyMoreInfo> GazConpanyMoreInfo
        {
            get { return this._GazConpanyMoreInfo; }
            set { this._GazConpanyMoreInfo = value; }
        }


        [XmlIgnore]
        public List<GazConpanyMoreInfo> Values
        {
            get
            {
                if (this._GazConpanyMoreInfo == null)
                {
                    //Console.WriteLine($"tagValues is null: {tagValues == null}");
                    return null;
                }
                else return this._GazConpanyMoreInfo.values;
            }
        }
        
    }*/

    public class GazConpanyInfo
    {
        /// <summary>
        /// Наименование компании
        /// </summary>
        [XmlElement("desc")]
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор для синхронизации в Справочнике Сотрудников(ЮЛ)
        /// </summary>
        [XmlElement("id")]
        public string Synctag { get; set; }
    }

    public class AnketFILE
    {
        /// <summary>
        /// Описание
        /// </summary>
        [XmlElement("title")]
        public string title { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [XmlElement("document")]
        public FileInfo FileInfo { get; set; }

        
    }
       
    public class FileInfo
    {

        /// <remarks/>
        [XmlAttributeAttribute("xlink")]
        public string LinkFile { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string FileName { get; set; }

    }

    

}



