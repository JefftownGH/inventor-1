using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventor;

namespace Vinogradov_Inventor
{
    public partial class Form1 : Form
    {

        /// ThisApplication - Объект для определения активного состояния Инвентора
        /// </summary>
        private Inventor.Application ThisApplication = null;
        /// <summary>
        /// Словарь для хранения ссылок на документы деталей
        /// </summary>
        private Dictionary<string, PartDocument> oPartDoc = new Dictionary<string, PartDocument>();
        /// <summary>
        /// Словарь для хранения ссылок на определения деталей
        /// </summary>
        private Dictionary<string, PartComponentDefinition> oCompDef = new
        Dictionary<string, PartComponentDefinition>();
        /// <summary>
        /// Словарь для хранения ссылок на инструменты создания деталей
        /// </summary>
        private Dictionary<string, TransientGeometry> oTransGeom = new
        Dictionary<string, TransientGeometry>();
        /// <summary>
        /// Словарь для хранения ссылок на транзакции редактирования
        /// </summary>
        private Dictionary<string, Transaction> oTrans = new Dictionary<string, Transaction>();
        /// <summary>
        /// Словарь для хранения имен сохраненных документов деталей
        /// </summary>
        private Dictionary<string, string> oFileName = new Dictionary<string, string>();
        private AssemblyDocument oAssemblyDocName;

        public static double d = 12, D = 32, B = 20, r = 1, r1 = 0.3;
        public static double n = 0, m = 0;
        public static int count = 12;
        public static double k = 12;

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            count = Convert.ToInt16(comboBox2.Text);
        }

        public Form1()
        {
            InitializeComponent();
            try
            {
                ThisApplication = (Inventor.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application");
                if (ThisApplication != null) label1.Text = "Inventor запущен.";
            }
            catch
            {
                label1.Text = "Запустите Inventor!";
            }
            this.comboBox1.Items.AddRange(new object[] { "236201", "246201", "266201", "236202", "246202", "266202", "236203", "246203", "266203" });
            comboBox1.Text = "236201";

            this.comboBox2.Items.AddRange(new object[] { "3", "6", "9" });
            comboBox2.Text = "9";

        }

        private void New_document_Name(string Name)
        {
            // Новый документ детали
            oPartDoc[Name] = (PartDocument)ThisApplication.Documents.Add(DocumentTypeEnum.kPartDocumentObject, ThisApplication.FileManager.GetTemplateFile(DocumentTypeEnum.kPartDocumentObject));
            // Новое определение
            oCompDef[Name] = oPartDoc[Name].ComponentDefinition;
            // Выбор инструментов
            oTransGeom[Name] = ThisApplication.TransientGeometry;
            // Создание транзакции
            oTrans[Name] = ThisApplication.TransactionManager.StartTransaction(
            ThisApplication.ActiveDocument, "Create Sample");
            // Имя файла
            oFileName[Name] = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ThisApplication = (Inventor.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application");
                if (ThisApplication != null) label1.Text = "Inventor запущен.";
            }
            catch
            {
                MessageBox.Show("Запустите Inventor!");
                return;
            }

            //Перевод мм в см
            D /= 10; d /= 10; B /= 10; r /= 10; r1 /= 10;

            //Sketch..
            New_document_Name("Подшипник");
            oPartDoc["Подшипник"].DisplayName = "Подшипник";
            PlanarSketch oSketch = oCompDef["Подшипник"].Sketches.Add(oCompDef["Подшипник"].WorkPlanes[3]);
            SketchPoint[] point = new SketchPoint[101];
            SketchArc[] arc = new SketchArc[101];
            SketchPoint[] center_arc = new SketchPoint[101];
            SketchLine[] lines = new SketchLine[101];
            SketchCircle[] circles = new SketchCircle[101];

            //Координаты точек вершин прямоугольника
            point[0] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, D/2));
            point[1] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, d/2));
            point[2] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, d/2));
            point[3] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, D/2));
            point[4] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, (D/2 - d/2)/3 + d/2));
            point[5] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, (D/2 - d/2)*2/3 + d/2));
            point[6] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, (D/2 - d/2)/3 + d/2));
            point[7] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, (D/2 - d/2)*2/3 + d/2));

            //Соединение верхнего прямоугольника
            lines[0] = oSketch.SketchLines.AddByTwoPoints(point[0], point[5]);
            lines[1] = oSketch.SketchLines.AddByTwoPoints(point[3], point[7]);
            lines[2] = oSketch.SketchLines.AddByTwoPoints(point[0], point[3]);
            lines[3] = oSketch.SketchLines.AddByTwoPoints(point[5], point[7]);

            //Содинение вершин нижнего прямоугольника
            lines[4] = oSketch.SketchLines.AddByTwoPoints(point[4], point[1]);
            lines[5] = oSketch.SketchLines.AddByTwoPoints(point[1], point[2]);
            lines[6] = oSketch.SketchLines.AddByTwoPoints(point[4], point[6]);
            lines[7] = oSketch.SketchLines.AddByTwoPoints(point[6], point[2]);

            //Ось вращения двух прямоугольников
            point[8] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, 0));
            point[9] = oSketch.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, 0));
            lines[8] = oSketch.SketchLines.AddByTwoPoints(point[8], point[9]);

            //Closing the sketch
            oTrans["Подшипник"].End();

            //Вращение
            Profile oProfile = default(Profile);
            oProfile = (Profile)oSketch.Profiles.AddForSolid();
            RevolveFeature revolvefeature = default(RevolveFeature);
            revolvefeature = oCompDef["Подшипник"].Features.RevolveFeatures.AddFull(oProfile, lines[8], PartFeatureOperationEnum.kJoinOperation);



            /*
             * Новый скетч 1
             */


            
            //Новый sketch с окружностями
            PlanarSketch oSketch1 = oCompDef["Подшипник"].Sketches.Add(oCompDef["Подшипник"].WorkPlanes[3]);
            Transaction oTrans1 = ThisApplication.TransactionManager.StartTransaction(ThisApplication.ActiveDocument, "Create Sample");

            //Окружности
            point[10] = oSketch1.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B/4, (D/2 + d/2)/2));
            point[11] = oSketch1.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B*3/4, (D/2 + d/2)/2));
            circles[0] = oSketch1.SketchCircles.AddByCenterRadius(point[10], (D/2-d/2)/3.6);
            circles[1] = oSketch1.SketchCircles.AddByCenterRadius(point[11], (D/2-d/2)/3.6);

            //Ось вращения подшипника
            point[12] = oSketch1.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, 0));
            point[13] = oSketch1.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, 0));
            lines[9] = oSketch1.SketchLines.AddByTwoPoints(point[12], point[13]);

            //Closing the sketch 1
            oTrans1.End();
            
            //Выдавливание выемки
            Profile oProfile1 = default(Profile);
            oProfile1 = (Profile)oSketch1.Profiles.AddForSolid();
            RevolveFeature revolvefeature1 = default(RevolveFeature);
            revolvefeature1 = oCompDef["Подшипник"].Features.RevolveFeatures.AddFull(oProfile1, lines[9], PartFeatureOperationEnum.kCutOperation);

            /*
             * New sketch 4
             */

            //New sketch 4
            PlanarSketch oSketch4 = oCompDef["Подшипник"].Sketches.Add(oCompDef["Подшипник"].WorkPlanes[3]);
            Transaction oTrans4 = ThisApplication.TransactionManager.StartTransaction(ThisApplication.ActiveDocument, "Create Sample");

            //Четырехугольник для вычетания выдавливания по краям подшипнника (слева)
            point[20] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, D / 2 * 0.7916875 - 0.1));
            point[21] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, D / 2 * 0.9275625));
            point[22] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B / 4, D / 2 * 0.861125));
            point[23] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B / 4, D / 2 * 0.7916875 - 0.1));
            lines[12] = oSketch4.SketchLines.AddByTwoPoints(point[20], point[21]);
            lines[13] = oSketch4.SketchLines.AddByTwoPoints(point[20], point[23]);
            lines[14] = oSketch4.SketchLines.AddByTwoPoints(point[21], point[22]);
            lines[15] = oSketch4.SketchLines.AddByTwoPoints(point[22], point[23]);

            point[26] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, D / 2 * 0.7916875 - 0.1));
            point[27] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, D / 2 * 0.9275625));
            point[28] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B*3/4, D / 2 * 0.861125));
            point[29] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B*3/4, D / 2 * 0.7916875 - 0.1));
            lines[17] = oSketch4.SketchLines.AddByTwoPoints(point[26], point[27]);
            lines[18] = oSketch4.SketchLines.AddByTwoPoints(point[27], point[28]);
            lines[19] = oSketch4.SketchLines.AddByTwoPoints(point[28], point[29]);
            lines[20] = oSketch4.SketchLines.AddByTwoPoints(point[29], point[26]);

            //ось
            point[24] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, 0));
            point[25] = oSketch4.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B, 0));
            lines[16] = oSketch4.SketchLines.AddByTwoPoints(point[24], point[25]);

            //Closing the sketch 4
            oTrans4.End();

            //Вычетание началось (слева)
            Profile oProfile4 = default(Profile);
            oProfile4 = (Profile)oSketch4.Profiles.AddForSolid();
            RevolveFeature revolveFeature4 = default(RevolveFeature);
            revolveFeature4 = oCompDef["Подшипник"].Features.RevolveFeatures.AddFull(oProfile4, lines[16], PartFeatureOperationEnum.kCutOperation);



            /*
             * New sketch 2
             */




            //Новый sketch с окружностями
            PlanarSketch oSketch2 = oCompDef["Подшипник"].Sketches.Add(oCompDef["Подшипник"].WorkPlanes[3]);
            Transaction oTrans2 = ThisApplication.TransactionManager.StartTransaction(ThisApplication.ActiveDocument, "Create Sample");

            //Задаю полуокружность!
            point[14] = oSketch2.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B/4, D / 2 * n));
            point[15] = oSketch2.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B/4, D/2 * m));
            lines[10] = oSketch2.SketchLines.AddByTwoPoints(point[14], point[15]);
            point[16] = oSketch2.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B/4, (D / 2 + d / 2) / 2));
            arc[0] = oSketch2.SketchArcs.AddByCenterStartEndPoint(point[16], point[15], point[14], false);


            //Closing the sketch 2
            oTrans2.End();


            //Выдавливание окружности 1
            Profile oProfile2 = default(Profile);
            oProfile2 = (Profile)oSketch2.Profiles.AddForSolid();
            RevolveFeature revolveFeature2 = default(RevolveFeature);
            revolveFeature2 = oCompDef["Подшипник"].Features.RevolveFeatures.AddFull(oProfile2, lines[10], PartFeatureOperationEnum.kJoinOperation);

            //New sketch 3
            PlanarSketch oSketch3 = oCompDef["Подшипник"].Sketches.Add(oCompDef["Подшипник"].WorkPlanes[3]);
            Transaction oTrans3 = ThisApplication.TransactionManager.StartTransaction(ThisApplication.ActiveDocument, "Create Sample");

            //Задаю окружность второго шарика
            point[17] = oSketch3.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B * 3 / 4, D / 2 * n));
            point[18] = oSketch3.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B * 3 / 4, D / 2 * m));
            lines[11] = oSketch3.SketchLines.AddByTwoPoints(point[17], point[18]);
            point[19] = oSketch3.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(B * 3 / 4, (D / 2 + d / 2) / 2));
            arc[1] = oSketch3.SketchArcs.AddByCenterStartEndPoint(point[19], point[17], point[18], false);

            //Closing the sketch 3
            oTrans3.End();

            //Вращение шарика
            Profile oProfile3 = default(Profile);
            oProfile3 = (Profile)oSketch3.Profiles.AddForSolid();
            RevolveFeature revolveFeature3 = default(RevolveFeature);
            revolveFeature3 = oCompDef["Подшипник"].Features.RevolveFeatures.AddFull(oProfile3, lines[11], PartFeatureOperationEnum.kJoinOperation);

            WorkAxis oAxis = oCompDef["Подшипник"].WorkAxes[1];
            ObjectCollection oObjCollection = ThisApplication.TransientObjects.CreateObjectCollection();
            oObjCollection.Add(revolveFeature3);
            CircularPatternFeature CircularPatternFeature = oCompDef["Подшипник"].Features.CircularPatternFeatures.Add(oObjCollection, oAxis, true, count, "360 degree", true, PatternComputeTypeEnum.kIdenticalCompute);

            WorkAxis oAxis1 = oCompDef["Подшипник"].WorkAxes[1];
            ObjectCollection oObjCollection1 = ThisApplication.TransientObjects.CreateObjectCollection();
            oObjCollection1.Add(revolveFeature2);
            CircularPatternFeature oCircularPatternFeature1 = oCompDef["Подшипник"].Features.CircularPatternFeatures.Add(oObjCollection1, oAxis1, true, count, "360 degree", true, PatternComputeTypeEnum.kIdenticalCompute);




            /*НАЧАЛО ВАЛА
             * НАЧАЛО ВАЛА
             * НАЧАЛО ВАЛА
             * НАЧАЛО ВАЛА
             */




            //New sketch 5
            PlanarSketch oSketch5 = oCompDef["Подшипник"].Sketches.Add(oCompDef["Подшипник"].WorkPlanes[3]);
            Transaction oTrans5 = ThisApplication.TransactionManager.StartTransaction(ThisApplication.ActiveDocument, "Create Sample");

            //Ось вала
            point[30] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, 0));
            point[31] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0.7, 0));
            lines[16] = oSketch5.SketchLines.AddByTwoPoints(point[30], point[31]);

            //Эскиз вала
            point[32] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, (((D/2 - d/2)/3 + d/2)+d/2)/2));
            point[33] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(0, d/2));
            lines[17] = oSketch5.SketchLines.AddByTwoPoints(point[32], point[33]);
            point[34] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(-0.1*k, (((D / 2 - d / 2) / 3 + d / 2) + d / 2) / 2));
            lines[18] = oSketch5.SketchLines.AddByTwoPoints(point[32], point[34]);
            point[35] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d(-0.1 * k, 0));
            lines[19] = oSketch5.SketchLines.AddByTwoPoints(point[34], point[35]);
            point[36] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d((B + 0.15)*k, d/2));
            lines[20] = oSketch5.SketchLines.AddByTwoPoints(point[33], point[36]);
            point[37] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d((B + 0.15) * k, 0.4 * k));
            lines[21] = oSketch5.SketchLines.AddByTwoPoints(point[36], point[37]);
            point[38] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d((B + 0.15) * k + 0.3*k, 0.4 * k));
            lines[22] = oSketch5.SketchLines.AddByTwoPoints(point[37], point[38]);
            point[39] = oSketch5.SketchPoints.Add(oTransGeom["Подшипник"].CreatePoint2d((B + 0.15) * k + 0.3 * k, 0));
            lines[23] = oSketch5.SketchLines.AddByTwoPoints(point[38], point[39]);
            lines[24] = oSketch5.SketchLines.AddByTwoPoints(point[39], point[35]);

            //Closing
            oTrans5.End();

            //Вращение вала
            Profile oProfile5 = default(Profile);
            oProfile5 = (Profile)oSketch5.Profiles.AddForSolid();
            RevolveFeature revolveFeature5 = default(RevolveFeature);
            revolveFeature5 = oCompDef["Подшипник"].Features.RevolveFeatures.AddFull(oProfile5, lines[16], PartFeatureOperationEnum.kJoinOperation);

            //Вращение вала
            /*Profile oProfile5 = default(Profile);
            oProfile5 = (Profile)oSketch5.Profiles.AddForSolid();
            RevolveFeature revolveFeature5 = default(RevolveFeature);
            revolveFeature5 = oCompDef["Подшипник"].Features.RevolveFeatures.AddFull(oProfile5, lines[16], PartFeatureOperationEnum.kJoinOperation);*/



            //см в мм
            D *= 10; d *= 10; B *= 10; r *= 10; r1 *= 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("C:/Program Files/Autodesk/Inventor2017/Bin/Inventor.exe");
                label1.Text = "Inventor запущен.";
            }
            catch
            {
                System.Diagnostics.Process.Start("C:/Program Files/Autodesk/Inventor 2017/Bin/Inventor.exe");
                label1.Text = "Inventor запущен.";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: D = 32; d = 12; B = 20; r = 1; r1 = 0.3; n = 0.861125; m = 0.513875; k = 1.07; break;
                case 1: D = 32; d = 12; B = 20; r = 1; r1 = 0.3; n = 0.861125; m = 0.513875; k = 1.07; break;
                case 2: D = 32; d = 12; B = 20; r = 1; r1 = 0.3; n = 0.861125; m = 0.513875; k = 1.07; break;
                case 3: D = 35; d = 15; B = 22; r = 1; r1 = 0.3; n = 0.87302857142; m = 0.555542857142; k = 1.11; break;
                case 4: D = 35; d = 15; B = 22; r = 1; r1 = 0.3; n = 0.87302857142; m = 0.555542857142; k = 1.11; break;
                case 5: D = 35; d = 15; B = 22; r = 1; r1 = 0.3; n = 0.87302857142; m = 0.555542857142; k = 1.11; break;
                case 6: D = 40; d = 17; B = 24; r = 1; r1 = 0.3; n = 0.8722; m = 0.5528; k = 1.15; break;
                case 7: D = 40; d = 17; B = 24; r = 1; r1 = 0.3; n = 0.8722; m = 0.5528; k = 1.15; break;
                case 8: D = 40; d = 17; B = 24; r = 1; r1 = 0.3; n = 0.8722; m = 0.5528; k = 1.15; break;
            }
            

            textBox1.Text = Convert.ToString(D);
            textBox2.Text = Convert.ToString(B);
            textBox3.Text = Convert.ToString(d);
            textBox4.Text = Convert.ToString(r);
            textBox5.Text = Convert.ToString(r1);

        }
    }
}
