﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFERender
{
    public class Element
    {
        public string type { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string isRequired { get; set; }
        public string defaultValue { get; set; }
        // public string Prows { get; set; }
        // public List<choice> choices { get; set; }
        public List<MultiText> MultipleText { get; set; }
        public List<PanelData> Panel { get; set; }
        public choicesByUrl choicesByUrl { get; set; }
        public List<choices> choices { get; set; }
        public string readOnly { get; set; }
        public string expression { get; set; }
        public string visibleIf { get; set; }
        public string inputType { get; set; }
        public string acceptedTypes { get; set; }
        public string maxSize { get; set; }
        public string CascadeControlName { get; set; }
        public string DepandantControlName { get; set; }
        public string ApiUrl { get; set; }
        public string ValueFiled { get; set; }
        public string DisplayFiled { get; set; }
        public string DbTableName { get; set; }
        public string Condition { get; set; }
        public string ApiType { get; set; }
        public string ApiParameter { get; set; }
        public string ValidationFiled { get; set; }
        public List<matrixdynamicData> matrixdynamicData { get; set; }
        public string calltype { get; set; }
        public string QueryString { get; set; }
        public string Session { get; set; }
        public List<matrixdynamicData> ColmatrixdropdownData { get; set; }
        public List<choices> RowmatrixdropdownData { get; set; }
        public string html { get; set; }
    }
    public class choices
    {
        public string value { get; set; }
        public string text { get; set; }
        public int Mindex { get; set; }
    }
    public class MultiText
    {
        public string Mtext { get; set; }
        public string MdefaultValue { get; set; }
        public string title { get; set; }
    }
    public class PanelData
    {
        public string type { get; set; }
        public string name { get; set; }
        public string title { get; set; }
    }
    public class choicesByUrl
    {
        public string valueName { get; set; }
        public string titleName { get; set; }
        public string url { get; set; }
        public string path { get; set; }
        public string type { get; set; }
    }
    public class matrixdynamicData
    {
        public string name { get; set; }
        public string title { get; set; }
        public string cellType { get; set; }
        public List<choices> matrixdropdownData { get; set; }
    }
}
