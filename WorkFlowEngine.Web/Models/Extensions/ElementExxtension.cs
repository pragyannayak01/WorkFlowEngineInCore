using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFEngine;

namespace WorkFlowEngine.Web.Models.Extensions
{
    public static class ElementExxtension
    {
        public static List<Element> GetObjectFromElements(this FormModel input)
        {
            JObject o = JObject.Parse(input.JSONSTRING);
            List<Element> objs = new List<Element>();
            var elementsP1 = o.SelectToken("pages[0].elements", false); //pages[0].elements[0].type
            var pagedata = o.SelectToken("pages[0]", false);
            if (pagedata.ToList().Count > 0)
            {
                //var pagelement = new Element();
                foreach (var item in elementsP1.Children())
                {
                    //((Newtonsoft.Json.Linq.JValue)o.SelectToken("pages[0].name", false)).Value
                    var element = new Element();
                    element.name = (string)item.SelectToken("name", false);
                    element.type = (string)item.SelectToken("type", false);
                    element.title = (string)item.SelectToken("title", false);
                    element.isRequired = (string)item.SelectToken("isRequired", false);
                    element.inputType = (string)item.SelectToken("inputType", false);
                    element.Page_ApiParameter = (string)o.SelectToken("pages[0].APIParam", false);
                    element.Page_Mapcontrol = (string)o.SelectToken("pages[0].MapControl", false);
                    element.Page_Endpoint = (string)o.SelectToken("pages[0].EndPoint", false);
                    element.DbTable_Name = (string)item.SelectToken("DbTableName", false);
                    element.ValueFiled = (string)item.SelectToken("ValueFiled", false);
                    element.DisplayFiled = (string)item.SelectToken("DisplayFiled", false);
                    if (element.title == null)
                    {
                        element.title = element.name.Replace(" ", string.Empty);
                    }

                    // if (element.type == "checkbox")
                    //{
                    //    element.defaultValue = string.Join(",", item.SelectToken("defaultValue", false));
                    //}
                    //else if (element.type != "multipletext")
                    //{
                    //    element.defaultValue = (string)item.SelectToken("defaultValue", false);
                    //}
                    if (element.type == "matrixdropdown")
                    {
                        //element.title = (string)item.SelectToken("rows", false).ToString();
                        //element.type = (string)item.SelectToken("cellType", false);
                        //if (element.type == null)
                        //{
                        //    element.type = "dropdown";
                        //}
                        var MatrixdynamicData = new List<matrixdynamicData>();
                        var json = item.SelectToken("columns", false);
                        foreach (var c in json.Children())
                        {
                            matrixdynamicData mt = new matrixdynamicData();
                            mt.name = (string)c.SelectToken("name", false);
                            mt.title = (string)c.SelectToken("title", false);
                            mt.cellType = (string)c.SelectToken("cellType", false);
                            MatrixdynamicData.Add(mt);
                        }
                        element.matrixdropdownData = MatrixdynamicData;
                    }
                    if (element.type == "multipletext")
                    {
                        //element.defaultValue = string.Join(",", item.SelectToken("defaultValue", false));
                        var MultipleText = new List<MultiText>();
                        var json = item.SelectToken("items", false);
                        //var jsonvalue = item.SelectToken("defaultValue", false);
                        foreach (var c in json.Children())
                        {
                            MultiText mt = new MultiText();
                            mt.Mtext = (string)c.SelectToken("name", false);
                            mt.title = (string)c.SelectToken("title", false);
                            //mt.MdefaultValue = (string)c.SelectToken("defaultValue", false);
                            MultipleText.Add(mt);
                        }
                        element.MultipleText = MultipleText;
                    }
                    if (element.type == "matrixdynamic")
                    {
                        var MatrixdynamicData = new List<matrixdynamicData>();
                        var json = item.SelectToken("columns", false);
                        foreach (var c in json.Children())
                        {
                            matrixdynamicData mt = new matrixdynamicData();
                            mt.name = (string)c.SelectToken("name", false);
                            mt.title = (string)c.SelectToken("title", false);
                            mt.cellType = (string)c.SelectToken("cellType", false);
                            MatrixdynamicData.Add(mt);
                        }
                        element.matrixdynamicData = MatrixdynamicData;
                    }

                    if (element.type == "panel")
                    {
                        var Panel = new List<PanelData>();
                        var json = item.SelectToken("elements", false);
                        foreach (var c in json.Children())
                        {
                            PanelData DPanel = new PanelData();
                            DPanel.name = (string)c.SelectToken("name", false);
                            DPanel.type = (string)c.SelectToken("type", false);
                            DPanel.title = (string)c.SelectToken("title", false);
                            Panel.Add(DPanel);
                            if (DPanel.type == "multipletext")
                            {
                                //element.defaultValue = string.Join(",", item.SelectToken("defaultValue", false));
                                var MultipleText = new List<MultiText>();
                                var jsonn = c.SelectToken("items", false);
                                //var jsonvalue = item.SelectToken("defaultValue", false);
                                foreach (var cc in jsonn.Children())
                                {
                                    MultiText mt = new MultiText();
                                    mt.Mtext = (string)cc.SelectToken("name", false);
                                    mt.title = (string)cc.SelectToken("title", false);
                                    //mt.MdefaultValue = (string)c.SelectToken("defaultValue", false);
                                    MultipleText.Add(mt);
                                }
                                element.MultipleText = MultipleText;
                            }
                        }
                        element.Panel = Panel;
                    }
                    objs.Add(element);
                }
            }
            return objs;
        }
    }

}
