using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Compiler
{
    public partial class Form1 : Form
    {
        Timer T ;
        List<Variable> Variables = new List<Variable>();
        Stack<Token> Tokens = new Stack<Token>();
        string[] Identifiers;
        string[] ReservedWords;
        string p = "";
        int BrcltsDone = 0;
        string t = "";
        List<string> Errors = new List<string>();
        int MouseIsHere = 0;
        
        public Form1()
        {
            InitializeComponent();
            T = new Timer();
            this.WindowState = FormWindowState.Maximized;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Load +=new EventHandler(Form1_Load);
            richTextBox1.KeyDown += new KeyEventHandler(richTextBox1_KeyDown);
            richTextBox1.MouseMove += new MouseEventHandler(richTextBox1_MouseMove);
            richTextBox1.MouseLeave += new EventHandler(richTextBox1_MouseLeave);
            T.Tick += new EventHandler(T_Tick);
            T.Start();
        }

        void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            MouseIsHere = 0;
        }

        void richTextBox1_MouseMove(object sender, MouseEventArgs e)
        {
            MouseIsHere = 1;
        }

        void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.OemOpenBrackets:
                    BrcltsDone = 1;
                                int SelectBack = richTextBox1.SelectionStart;
                    richTextBox1.SelectionStart = 0;
                 richTextBox1.SelectionLength = richTextBox1.Text.Length;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.Text += "\r\n";
                    richTextBox1.Text += "\r\n";
                    richTextBox1.Text += '}';

                                richTextBox1.SelectionStart = SelectBack;
                                richTextBox1.SelectionLength = 0;
                    break;
            }
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        void SetArrays()
        {
            Identifiers = new string[] { "int", "float", "string", "double", "bool", "double", "char" };
            ReservedWords = new string[] { "int", "float", "string", "double", "bool", "double", "char", "for", "while", "if", "break", "return", "continue", "end", "do" };
        }

        void T_Tick(object sender, EventArgs e)
        {
            int x = richTextBox1.GetFirstCharIndexOfCurrentLine();
            int currentline = richTextBox1.GetLineFromCharIndex(x) + 1;
            textBox1.Text = currentline.ToString();
            if (MouseIsHere == 0)
            {
                SetLineDesign();
                Colourit();
            }
            RichBoxReadLineByLine();
            DesignErrors();
            this.Text = MouseIsHere.ToString();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
        void SetRichBoxes()
        {
            richTextBox2.SelectionProtected = true;
            richTextBox2.ReadOnly = true;
            richTextBox3.ReadOnly = true;
            richTextBox3.SelectionProtected = true;
            textBox1.ReadOnly = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetRichBoxes();
            SetArrays();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RichBoxReadLineByLine();
            DrawMemory();
        }

        void RichBoxReadLineByLine()
        {
            Variables.Clear();
            richTextBox3.Clear();
            richTextBox4.Clear();
            for (int i = 0; i<richTextBox1.Lines.Length ; i++)
            {
                string x = richTextBox1.Lines[i].ToString();
                if (x != "")
                {
                    if (x[x.Length - 1] == ';')
                    {
                        if (IsDeclareVariable(x,i))
                        {
                            CreateVariable(x,i);
                        }
                        if (IsLogicline(x, i))
                        {
                            string SV = "";
                            int UU = 0;
                            while(x[UU] != ' ')
                            {
                                SV += x[UU];
                                UU++;
                            }
                            ChangeVariable(x,i,SV);
                        }
                    }
                    else
                    {
                        if(x[x.Length - 1] != ')')
                        {
                            int koko = i + 1;
                            string eror = "Missing SemiColon in line " + koko;
                            Errors.Add(eror); // OS
                        }
                    }

                    if (x[x.Length - 1] == ')' && x[0] == 'i')
                    {
                        bool ifCon = Mainif(x);
                        if (ifCon)
                        {
                            if(richTextBox1.Lines.Length > i + 1)
                            {
                                if (richTextBox1.Lines[i + 1] == "{")
                                {
                                    i++;
                                    while (richTextBox1.Lines[i] != "}" && BrcltsDone == 1)
                                    {
                                        x = richTextBox1.Lines[i].ToString();
                                        if (x != "")
                                        {
                                            if (x[x.Length - 1] == ';')
                                            {
                                                if (IsDeclareVariable(x, i))
                                                {
                                                    CreateVariable(x, i);
                                                }
                                                if (IsLogicline(x, i))
                                                {
                                                    string SV = "";
                                                    int UU = 0;
                                                    while (x[UU] != ' ')
                                                    {
                                                        SV += x[UU];
                                                        UU++;
                                                    }
                                                    ChangeVariable(x, i, SV);
                                                }
                                            }
                                        }
                                        i++;
                                    }
                                }
                            }
                        
                         }
                        else
                        {
                            while (richTextBox1.Lines[i] != "}" && BrcltsDone == 1)
                                i++;
                        }
                    }
                    if (x[x.Length - 1] == ')' && x[0] == 'w')
                    {
                        bool LoopCon = false;
                        string LoopHead = x;
                        checkloop(x,ref LoopCon);
                        int iback = i;
                        while (LoopCon)
                        {
                            i = iback;
                            if (LoopCon)
                            {
                                while (richTextBox1.Lines[i] != "}" && BrcltsDone == 1)
                                {
                                    string z = richTextBox1.Lines[i].ToString();
                                    if (z != "")
                                    {
                                        if (z[z.Length - 1] == ';')
                                        {
                                            if (IsDeclareVariable(z, i))
                                            {
                                                CreateVariable(z, i);
                                            }
                                            if (IsLogicline(z, i))
                                            {
                                                string SV = "";
                                                int UU = 0;
                                                while (z[UU] != ' ')
                                                {
                                                    SV += z[UU];
                                                    UU++;
                                                }
                                                ChangeVariable(z, i, SV);
                                            }
                                        }
                                    }
                                    i++;
                                }
                            }
                            LoopCon = false;
                            checkloop(x, ref LoopCon);
                        }
                    }
                }

            }
            DrawMemory();
        }

        bool Mainif(string text)
        {
            
            int i = 0;
            bool Condi = false;
            while (text[i] != '(')
                i++;

            i += 2;
            while (text[i] != ')')
            {
                string OneCondition = "if ( ";
                while (text[i] != '&' && text[i] != '|' && text[i] != ')')
                {
                    OneCondition += text[i];
                    i++;
                }
                OneCondition += ')';
                checkif(OneCondition,ref Condi);
                if (text[i] == '&')
                {
                    i += 2;
                    if (Condi == false)
                        return false;
                        break;
                }
                if (text[i] == '|')
                {
                    i += 2;
                    if (Condi == true)
                        return true;
                        break;
                }
              }
            if(Condi)
                return true;


            return false;
        }

        void checkif(string text, ref bool Condi)
        {
            t = "";
            p = "";
            int i = 0;
            while (text[i] != ' ')
            {
                p += text[i];
                i++;
            }
            if (p == "if")
            {
                while (text[i] == ' ')
                {
                    i++;
                }
                if (text[i] == '(')
                {
                    i++;
                }
                i++;
                while (text[i] != ' ' && text[i] >= 97 && text[i] <= 122)
                {
                    t += text[i];
                    i++;
                }
                int g = 0;
                int numone = 0;
                while (text[i] != ' ' && text[i] >= 48 && text[i] <= 57)
                {
                    t += text[i];
                    i++;
                    g = 1;
                }
                if (g == 1)
                {
                    numone = Int32.Parse(t);
                }
                int z = 0;
                int y = 0;
                int m = 0;
                int n = 0;
                for (int k = 0; k < Variables.Count; k++)
                {
                    if (t == Variables[k].name)
                    {
                        z = 1;
                        n = k;
                    }
                }
                i += 1;
                int o = i;
                if (text[i] == '=' && text[i + 1] == '=' || text[i] == '!' && text[i + 1] == '=' || text[i] == '<' && text[i + 1] == '=' || text[i] == '>'
                    && text[i + 1] == '=')
                {
                    o += 3;
                }
                if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                {
                    o += 2;
                }
                t = "";
                while (text[o] != ' ' && text[o] >= 97 && text[o] <= 122)
                {
                    t += text[o];
                    o++;
                }
                int h = 0;
                int numtwo = 0;
                while (text[o] != ' ' && text[o] >= 48 && text[o] <= 57)
                {
                    t += text[o];
                    o++;
                    h = 1;
                }
                if (h == 1)
                {
                    numtwo = Int32.Parse(t);
                }
                for (int k = 0; k < Variables.Count; k++)
                {
                    if (t == Variables[k].name)
                    {
                        y = 1;
                        m = k;
                    }
                }
                o += 1;
                int l = 0;
                if (text[o] == ')')
                {
                    l = 1;
                }
                if (z == 1 && y == 1 && l == 1)
                {
                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator)
                        {
                            case '>':
                                if (q > w)
                                {
                                    Condi = true;
                                }
                                break;
                            case '<':
                                if (q < w)
                                {
                                    Condi = true;
                                }
                                break;
                            case '%':
                                if (q % w == 0)
                                {
                                    Condi = true;
                                }
                                break;
                        }
                    }
                    if (text[i] == '=' && text[i + 1] == '=' || text[i] == '!' && text[i + 1] == '=' || text[i] == '<' && text[i + 1] == '=' || text[i] == '>'
                        && text[i + 1] == '=')
                    {
                        string Operator2 = "";
                        Operator2 += text[i];
                        Operator2 += text[i + 1];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator2)
                        {
                            case "==":
                                if (q == w)
                                {
                                    Condi = true;
                                }
                                break;
                            case "!=":
                                if (q != w)
                                {
                                    Condi = true;
                                }
                                break;
                            case ">=":
                                if (q >= w)
                                {
                                    Condi = true;
                                }
                                break;
                            case "<=":
                                if (q <= w)
                                {
                                    Condi = true;
                                }
                                break;
                        }

                    }

                }
                if (g == 1 && h == 1 && l == 1)
                {
                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        switch (Operator)
                        {
                            case '>':
                                if (numone > numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case '<':
                                if (numone < numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case '%':
                                if (numone % numtwo == 0)
                                {
                                    Condi = true;
                                }
                                break;
                        }
                    }
                    if (text[i] == '=' && text[i + 1] == '=' || text[i] == '!' && text[i + 1] == '=' || text[i] == '<' && text[i + 1] == '=' || text[i] == '>'
                        && text[i + 1] == '=')
                    {
                        string Operator2 = "";
                        Operator2 += text[i];
                        Operator2 += text[i + 1];
                        switch (Operator2)
                        {
                            case "==":
                                if (numone == numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case "!=":
                                if (numone != numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case ">=":
                                if (numone >= numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case "<=":
                                if (numone <= numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                        }

                    }
                }
                if (z == 1 && h == 1 && l == 1)
                {

                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator)
                        {
                            case '>':
                                if (q > numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case '<':
                                if (q < numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case '%':
                                if (q % numtwo == 0)
                                {
                                    Condi = true;
                                }
                                break;
                        }
                    }
                    if (text[i] == '=' && text[i + 1] == '=' || text[i] == '!' && text[i + 1] == '=' || text[i] == '<' && text[i + 1] == '=' || text[i] == '>'
                        && text[i + 1] == '=')
                    {
                        string Operator2 = "";
                        Operator2 += text[i];
                        Operator2 += text[i + 1];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator2)
                        {
                            case "==":
                                if (q == numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case "!=":
                                if (q != numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case ">=":
                                if (q >= numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                            case "<=":
                                if (q <= numtwo)
                                {
                                    Condi = true;
                                }
                                break;
                        }
                    }

                }

                if (g == 1 && y == 1 && l == 1)
                {
                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator)
                        {
                            case '>':
                                if (numone > w)
                                {
                                    Condi = true;
                                }
                                break;
                            case '<':
                                if (numone < w)
                                {
                                    Condi = true;
                                }
                                break;
                            case '%':
                                if (numone % w == 0)
                                {
                                    Condi = true;
                                }
                                break;
                        }
                    }
                    if (text[i] == '=' && text[i + 1] == '=' || text[i] == '!' && text[i + 1] == '=' || text[i] == '<' && text[i + 1] == '=' || text[i] == '>'
                        && text[i + 1] == '=')
                    {
                        string Operator2 = "";
                        Operator2 += text[i];
                        Operator2 += text[i + 1];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator2)
                        {
                            case "==":
                                if (numone == w)
                                {
                                    Condi = true;
                                }
                                break;
                            case "!=":
                                if (numone != w)
                                {
                                    Condi = true;
                                }
                                break;
                            case ">=":
                                if (numone >= w)
                                {
                                    Condi = true;
                                }
                                break;
                            case "<=":
                                if (numone <= w)
                                {
                                    Condi = true;
                                }
                                break;
                        }
                    }

                }
            }
        }

        bool IsDeclareVariable(string text,int LineNo)
        {
            if (text[3] != '=')
            {
                string DataType = "";
                string Vname = "";
                int pos = 0;
                Token pnn;
                if (IsIdentifier(text, ref DataType, LineNo))
                {
                    pnn = new Token();
                    pnn.Type = "I";
                    pnn.value = DataType;
                    Tokens.Push(pnn);
                    if (IsNotReservedWord(text, ref Vname, ref pos, LineNo))
                    {
                        pnn = new Token();
                        pnn.Type = "V";
                        pnn.value = Vname;
                        Tokens.Push(pnn);
                        if (text[pos] == ';')
                            return true;
                        int x = 0;
                        float f = 0;
                        double k = 0;
                        bool z = false;
                        char r = ' ';
                        if (text[pos + 1] == '=' && DataType == "char" || text[pos + 1] == '=' && DataType == "string")
                        {
                            int St = pos+4;
                            string j="";
                            while (text[St] != '"')
                            {
                                 j+= text[St];
                                 St++;
                            }
                            pnn = new Token();
                            pnn.Type = "N";
                            pnn.value = j;
                            Tokens.Push(pnn);
                            return true;
                        }

                        if (text[pos + 1] == '=' && DataType != "char" && DataType != "string")
                        {
                            char Operator = ' ';
                            int S = pos;
                            int FirstNoFlag = 0;

                            while (true)
                            {
                                if (text[S + 3] >= 48 && text[S + 3] <= 57)
                                {
                                    string No = "";
                                    for (S = S + 3; text[S] != ' ' && text[S] != ';'; S++)
                                    {
                                        No += text[S];
                                    }
                                    pnn = new Token();
                                    pnn.Type = "Num";
                                    pnn.value = No;
                                    Tokens.Push(pnn);
                                    if (FirstNoFlag == 0)
                                    {
                                        FirstNoFlag = 1;
                                        switch (DataType)
                                        {
                                            case "int":
                                                x = Int32.Parse(No);
                                                break;
                                            case "float":
                                                f = float.Parse(No);
                                                break;
                                            case "double":
                                                k = double.Parse(No);
                                                break;
                                            case "bool":
                                                z = bool.Parse(No);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (Operator)
                                        {
                                            case '+':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x += Int32.Parse(No);
                                                        break;
                                                    case "float":
                                                        f += float.Parse(No);
                                                        break;
                                                    case "double":
                                                        k += double.Parse(No);
                                                        break;
                                                }
                                                break;
                                            case '-':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x -= Int32.Parse(No);
                                                        break;
                                                    case "float":
                                                        f -= float.Parse(No);
                                                        break;
                                                    case "double":
                                                        k -= double.Parse(No);
                                                        break;
                                                }
                                                break;
                                            case '/':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x /= Int32.Parse(No);
                                                        break;
                                                    case "float":
                                                        f /= float.Parse(No);
                                                        break;
                                                    case "double":
                                                        k /= double.Parse(No);
                                                        break;
                                                }
                                                break;
                                            case '*':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x *= Int32.Parse(No);
                                                        break;
                                                    case "float":
                                                        f *= float.Parse(No);
                                                        break;
                                                    case "double":
                                                        k *= double.Parse(No);
                                                        break;
                                                }
                                                break;
                                            case '%':

                                                break;
                                        }
                                    }

                                    if (text[S] == ';')
                                        break;

                                    else
                                    {
                                        Operator = text[S + 1];
                                        pnn = new Token();
                                        pnn.value = Operator.ToString();
                                        pnn.Type = "Op";
                                        Tokens.Push(pnn);
                                    }
                                }

                                if (text[S + 3] >= 97 && text[S + 3] <= 122)
                                {
                                    S = S + 3;
                                    string tempvar = "";
                                    string tempvar2 = "";
                                    while (text[S] != ' ' && text[S] != ';')
                                    {
                                        tempvar += text[S];
                                        S++;
                                    }
                                    int FLAG = 0;
                                    for (int Q = 0; Q < Variables.Count; Q++)
                                    {
                                        if (Variables[Q].name == tempvar)
                                        {
                                            FLAG = 1;
                                            tempvar2 = tempvar;
                                            tempvar = Variables[Q].value;
                                            break;
                                        }
                                    }
                                    if (FLAG == 0)
                                    {
                                        int koko = LineNo+1;
                                        string eror = "Variable dosen't exist " + koko ;
                                        Errors.Add(eror);
                                        return false;
                                    }
                                    pnn = new Token();
                                    pnn.Type = "CV";
                                    pnn.value = tempvar2;
                                    Tokens.Push(pnn);
                                    if (FirstNoFlag == 0)
                                    {
                                        FirstNoFlag = 1;
                                        switch (DataType)
                                        {
                                            case "int":
                                                x = Int32.Parse(tempvar);
                                                break;
                                            case "float":
                                                f = float.Parse(tempvar);
                                                break;
                                            case "double":
                                                k = double.Parse(tempvar);
                                                break;
                                            case "bool":
                                                z = bool.Parse(tempvar);
                                                break;
                                            case "char":
                                                r = char.Parse(tempvar);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (Operator)
                                        {
                                            case '+':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x += Int32.Parse(tempvar);
                                                        break;
                                                    case "float":
                                                        f += float.Parse(tempvar);
                                                        break;
                                                    case "double":
                                                        k += double.Parse(tempvar);
                                                        break;
                                                }
                                                break;
                                            case '-':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x -= Int32.Parse(tempvar);
                                                        break;
                                                    case "float":
                                                        f -= float.Parse(tempvar);
                                                        break;
                                                    case "double":
                                                        k -= double.Parse(tempvar);
                                                        break;

                                                }
                                                break;
                                            case '/':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x /= Int32.Parse(tempvar);
                                                        break;
                                                    case "float":
                                                        f /= float.Parse(tempvar);
                                                        break;
                                                    case "double":
                                                        k /= double.Parse(tempvar);
                                                        break;
                                                }
                                                break;
                                            case '*':
                                                switch (DataType)
                                                {
                                                    case "int":
                                                        x *= Int32.Parse(tempvar);
                                                        break;
                                                    case "float":
                                                        f *= float.Parse(tempvar);
                                                        break;
                                                    case "double":
                                                        k *= double.Parse(tempvar);
                                                        break;
                                                }
                                                break;
                                            case '%':

                                                break;
                                        }
                                    }

                                    if (text[S] == ';')
                                        break;

                                    else
                                    {
                                        Operator = text[S + 1];
                                    }

                                }
                            }
                        }
                        pnn = new Token();
                        pnn.Type = "N";
                        switch (DataType)
                        {
                            case "int":
                                pnn.value = x.ToString();
                                break;

                            case "float":
                                pnn.value = f.ToString();
                                break;

                            case "double":
                                pnn.value = k.ToString();
                                break;

                            case "bool":
                                pnn.value = z.ToString();
                                break;
                            case "char":
                                pnn.value = r.ToString();
                                break;
                            case "string":

                                break;
                        }
                        Tokens.Push(pnn);
                        return true;
                    }
                }
            }
            return false;
        }

        bool IsLogicline(string text, int LineNo)
        {
            if (text[3] == '=')
            {
                int S = 0;
                Token pnn;
                string DataType = "";
                int FirstNoFlag = 0;
                char Operator = ' ';
                int x = 0;
                float f = 0;
                double k = 0;
                bool z = false;
                string Temp = "";
                while (text[S] != ' ')
                {
                    Temp += text[S];
                    S++;
                }
                S++;

                if (IsVariableExist(Temp, LineNo,ref DataType))
                {
                    while (true)
                    {
                        if (text[S + 3] >= 48 && text[S + 3] <= 57)
                        {
                            string No = "";
                            for (S = S + 3; text[S] != ' ' && text[S] != ';'; S++)
                            {
                                No += text[S];
                            }

                            pnn = new Token();
                            pnn.Type = "Num";
                            pnn.value = No;
                            Tokens.Push(pnn);
                            if (FirstNoFlag == 0)
                            {
                                FirstNoFlag = 1;
                                switch (DataType)
                                {
                                    case "int":
                                        x = Int32.Parse(No);
                                        break;
                                    case "float":
                                        f = float.Parse(No);
                                        break;
                                    case "double":
                                        k = double.Parse(No);
                                        break;
                                    case "bool":
                                        z = bool.Parse(No);
                                        break;
                                }
                            }
                            else
                            {
                                switch (Operator)
                                {
                                    case '+':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x += Int32.Parse(No);
                                                break;
                                            case "float":
                                                f += float.Parse(No);
                                                break;
                                            case "double":
                                                k += double.Parse(No);
                                                break;
                                        }
                                        break;
                                    case '-':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x -= Int32.Parse(No);
                                                break;
                                            case "float":
                                                f -= float.Parse(No);
                                                break;
                                            case "double":
                                                k -= double.Parse(No);
                                                break;
                                        }
                                        break;
                                    case '/':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x /= Int32.Parse(No);
                                                break;
                                            case "float":
                                                f /= float.Parse(No);
                                                break;
                                            case "double":
                                                k /= double.Parse(No);
                                                break;
                                        }
                                        break;
                                    case '*':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x *= Int32.Parse(No);
                                                break;
                                            case "float":
                                                f *= float.Parse(No);
                                                break;
                                            case "double":
                                                k *= double.Parse(No);
                                                break;
                                        }
                                        break;
                                    case '%':

                                        break;
                                }
                            }

                            if (text[S] == ';')
                                break;

                            else
                            {
                                Operator = text[S + 1];
                                pnn = new Token();
                                pnn.value = Operator.ToString();
                                pnn.Type = "Op";
                                Tokens.Push(pnn);
                            }
                        }

                        if (text[S + 3] >= 97 && text[S + 3] <= 122)
                        {
                            S = S + 3;
                            string tempvar = "";
                            string tempvar2 = "";
                            while (text[S] != ' ' && text[S] != ';')
                            {
                                tempvar += text[S];
                                S++;
                            }
                            int FLAG = 0;
                            for (int Q = 0; Q < Variables.Count; Q++)
                            {
                                if (Variables[Q].name == tempvar)
                                {
                                    FLAG = 1;
                                    tempvar2 = tempvar;
                                    tempvar = Variables[Q].value;
                                    break;
                                }
                            }
                            if (FLAG == 0)
                            {
                                int koko = LineNo+1;
                                string eror = "Variable dosen't exist " + koko ;
                                Errors.Add(eror);
                                return false;
                            }
                            pnn = new Token();
                            pnn.Type = "CV";
                            pnn.value = tempvar2;
                            Tokens.Push(pnn);
                            if (FirstNoFlag == 0)
                            {
                                FirstNoFlag = 1;
                                switch (DataType)
                                {
                                    case "int":
                                        x = Int32.Parse(tempvar);
                                        break;
                                    case "float":
                                        f = float.Parse(tempvar);
                                        break;
                                    case "double":
                                        k = double.Parse(tempvar);
                                        break;
                                    case "bool":
                                        z = bool.Parse(tempvar);
                                        break;
                                }
                            }
                            else
                            {
                                switch (Operator)
                                {
                                    case '+':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x += Int32.Parse(tempvar);
                                                break;
                                            case "float":
                                                f += float.Parse(tempvar);
                                                break;
                                            case "double":
                                                k += double.Parse(tempvar);
                                                break;
                                        }
                                        break;
                                    case '-':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x -= Int32.Parse(tempvar);
                                                break;
                                            case "float":
                                                f -= float.Parse(tempvar);
                                                break;
                                            case "double":
                                                k -= double.Parse(tempvar);
                                                break;

                                        }
                                        break;
                                    case '/':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x /= Int32.Parse(tempvar);
                                                break;
                                            case "float":
                                                f /= float.Parse(tempvar);
                                                break;
                                            case "double":
                                                k /= double.Parse(tempvar);
                                                break;
                                        }
                                        break;
                                    case '*':
                                        switch (DataType)
                                        {
                                            case "int":
                                                x *= Int32.Parse(tempvar);
                                                break;
                                            case "float":
                                                f *= float.Parse(tempvar);
                                                break;
                                            case "double":
                                                k *= double.Parse(tempvar);
                                                break;
                                        }
                                        break;
                                    case '%':

                                        break;
                                }
                            }

                            if (text[S] == ';')
                                break;

                            else
                            {
                                Operator = text[S + 1];
                                pnn = new Token();
                                pnn.value = Operator.ToString();
                                pnn.Type = "Op";
                                Tokens.Push(pnn);
                            }
                        }

                    }
                    pnn = new Token();
                    pnn.Type = "N";
                    switch (DataType)
                    {
                        case "int":
                            pnn.value = x.ToString();
                            break;

                        case "float":
                            pnn.value = f.ToString();
                            break;

                        case "double":
                            pnn.value = k.ToString();
                            break;

                        case "bool":
                            pnn.value = z.ToString();
                            break;
                    }
                    pnn.value = x.ToString();
                    Tokens.Push(pnn);
                    return true;
                }
            }
            else
                return false;

            return false;
        }

        bool IsVariableExist(string text,int LineNo,ref string DataType)
        {
            int i = 0;

            for (i = 0; i < Variables.Count; i++)
            {
                if (Variables[i].name == text)
                {
                    DataType = Variables[i].type;
                    return true;
                }
            }
            return false;
        }

        void DesignErrors()
        {
            richTextBox4.Clear();

            for (int i = 0; i < Errors.Count; i++)
            {
                richTextBox4.AppendText(Errors[i]);
                richTextBox4.AppendText("\r\n");
            }

            Errors.Clear();
        }

        bool IsIdentifier(string text, ref string DataType,int LineNo)
        {
            bool IsIdent = false;
            
            int i = 0;
            while (text[i] != ' ')
            {
                DataType += text[i];
                i++;
            }

            for (i = 0; i < Identifiers.Length; i++)
            {
                if (DataType == Identifiers[i])
                {
                    IsIdent = true;
                }
            }
            if (IsIdent == false)
            {
                int koko = LineNo + 1;
                string eror = "Non exist identifer in line " + koko;
                Errors.Add(eror);
            }

                return IsIdent;
        }

        bool IsNotReservedWord(string text,ref string name, ref int pos,int LineNo)
        {
            int i = 0;
            
            while (text[i] != ' ')
            {
                i++;
            }
            i++;
            while (text[i] != ' ' && text[i] != ';')
            {
                    name += text[i];
                    i++;   
            }
            pos = i;
            for (i = 0; i < ReservedWords.Length; i++)
            {
                if(name == ReservedWords[i])
                {
                    int koko = LineNo + 1;
                    string eror = "Reserved word using as variable name in line " + koko;
                    Errors.Add(eror);
                    return false;
                }
            }

            return true;
        }
        
        void SetLineDesign()
        {
            string[] lines = richTextBox1.Lines;
            int SelectBack = richTextBox1.SelectionStart;
            int i = 0;
            for (int j = 0; j < lines.Length; j++)
            {

                string line = lines[j];
                string newline = "";
                if (line != "")
                {
                    if (line[line.Length - 1] == ';')
                    {
                        int jojo = 0;
                        i = 0;
                        while (jojo < 1)
                        {
                            while (line[i] != ' ')
                            {
                                //takedatatype
                                newline += line[i];
                                i++;
                            }
                            newline += ' ';
                            while (line[i] == ' ')
                            {
                                i++;
                            }

                            while (line[i] != '=' && line[i] != ' ' && line[i] != ';')
                            {
                                //takename
                                newline += line[i];
                                i++;
                            }
                            if (line[i] != ';')
                            {
                                newline += ' ';
                                newline += '=';
                                newline += ' ';
                                while (line[i] == ' ')
                                {
                                    i++;
                                }
                                i++;
                                while (true)
                                {
                                    while (line[i] == ' ')
                                    {
                                        i++;
                                    }

                                    while (line[i] != ';' && line[i] != ' ')
                                    {
                                        newline += line[i];
                                        i++;
                                    }

                                    if (line[i] == ';')
                                        break;
                                    else
                                    {
                                        int AnotherIndex = i;
                                        int F = 0;
                                        while (AnotherIndex<line.Length)
                                        {
                                            if(line[AnotherIndex] != ' ' && line[AnotherIndex] != ';')
                                            {
                                                F=1;
                                                break;
                                            }
                                            AnotherIndex++;
                                        }
                                        if(F==1)
                                            newline += " ";
                                    }
                                }


                                newline += ';';
                                jojo++;
                            }
                            else
                            {
                                newline += ';';
                                jojo++;
                            }
                            //MessageBox.Show(newline);
                            lines[j] = newline;
                        }
                    }
                }
            }
            richTextBox1.Lines = lines;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;

                   
        }

        void DrawMemory()
        {
            richTextBox2.Clear(); 
            for (int i = 0; i < Variables.Count; i++)
            {
               string output = Variables[i].name + '=' + Variables[i].value;
               if (i == 0)
               {
                   richTextBox2.Text += output;
               }
               else
               {
                   richTextBox2.Text += "\r\n";
                   richTextBox2.Text += output;
               }
            }
        }

        void CreateVariable(string text,int LineNo)
        {
            Variable pnn = new Variable();
            
            richTextBox3.AppendText("Line "+ (LineNo+1) + " : ");
            richTextBox3.AppendText("\r\n");
            while (Tokens.Count>0)
            {
                
                Token Temp = Tokens.Pop();
                switch (Temp.Type)
                {
                    case "I":
                        richTextBox3.AppendText(Temp.value + " Is " + "Identifer");
                        richTextBox3.AppendText("\r\n");
                        pnn.type = Temp.value;
                        break;
                    case "V":
                        richTextBox3.AppendText(Temp.value + " Is " + "Variable");
                        richTextBox3.AppendText("\r\n");
                        pnn.name = Temp.value;
                        break;
                    case "CV":
                        richTextBox3.AppendText(Temp.value + " Is " + "Variable");
                        richTextBox3.AppendText("\r\n");
                        break;
                    case "N":
                        pnn.value = Temp.value;
                        break;
                    case "Num":
                        richTextBox3.AppendText(Temp.value + " Is " + "Number");
                        richTextBox3.AppendText("\r\n");
                        break;
                    case "Op":
                        richTextBox3.AppendText(Temp.value + " Is " + "Operator");
                        richTextBox3.AppendText("\r\n");
                        break;
                }
               
            }
            richTextBox3.AppendText("--------------------------------");
            richTextBox3.AppendText("\r\n");
            Variables.Add(pnn);
        }

        void checkloop(string text, ref bool LoopCon)
        {
            int i = 0;
            p = "";
            t = "";

            while (text[i] != ' ')
            {
                p += text[i];
                i++;
            }
            if (p == "while")
            {
                while (text[i] == ' ')
                {
                    i++;
                }
                if (text[i] == '(')
                {
                    i++;
                }
                i++;
                while (text[i] != ' ' && text[i] >= 97 && text[i] <= 122)
                {
                    t += text[i];
                    i++;
                }
                int g = 0;
                int numone = 0;
                while (text[i] != ' ' && text[i] >= 48 && text[i] <= 57)
                {
                    t += text[i];
                    i++;
                    g = 1;
                }
                if (g == 1)
                {
                    numone = Int32.Parse(t);
                }
                int z = 0;
                int y = 0;
                int m = 0;
                int n = 0;
                for (int k = 0; k < Variables.Count; k++)
                {
                    if (t == Variables[k].name)
                    {
                        z = 1;
                        n = k;
                    }
                }
                i += 1;
                int o = i;
                if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                {
                    o += 2;
                }
                t = "";
                while (text[o] != ' ' && text[o] >= 97 && text[o] <= 122)
                {
                    t += text[o];
                    o++;
                }
                int h = 0;
                int numtwo = 0;
                while (text[o] != ' ' && text[o] >= 48 && text[o] <= 57)
                {
                    t += text[o];
                    o++;
                    h = 1;
                }
                if (h == 1)
                {
                    numtwo = Int32.Parse(t);
                }
                for (int k = 0; k < Variables.Count; k++)
                {
                    if (t == Variables[k].name)
                    {
                        y = 1;
                        m = k;
                    }
                }
                o += 1;
                int l = 0;
                if (text[o] == ')')
                {
                    l = 1;
                }
                if (z == 1 && y == 1 && l == 1)
                {
                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator)
                        {
                            case '>':
                                if (q > w)
                                {
                                    LoopCon = true;
                                }
                                break;
                            case '<':
                                if (q < w)
                                {
                                    LoopCon = true;
                                }
                                break;
                            case '%':
                                if (q % w == 0)
                                {
                                    LoopCon = true;
                                }
                                break;
                        }
                    }
                }
                if (g == 1 && h == 1 && l == 1)
                {
                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        switch (Operator)
                        {
                            case '>':
                                if (numone > numtwo)
                                {

                                    LoopCon = true;
                                }
                                break;
                            case '<':
                                if (numone < numtwo)
                                {
                                    LoopCon = true;
                                }
                                break;
                            case '%':
                                if (numone % numtwo == 0)
                                {
                                    LoopCon = true;
                                }
                                break;
                        }
                    }
                }
                if (z == 1 && h == 1 && l == 1)
                {

                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator)
                        {
                            case '>':
                                if (q > numtwo)
                                {
                                    LoopCon = true;
                                }
                                break;
                            case '<':
                                if (q < numtwo)
                                {
                                    LoopCon = true;
                                }
                                break;
                            case '%':
                                if (q % numtwo == 0)
                                {
                                    LoopCon = true;
                                }
                                break;
                        }
                    }
                }
                if (g == 1 && y == 1 && l == 1)
                {
                    if (text[i] == '<' || text[i] == '>' || text[i] == '%')
                    {
                        char Operator = text[i];
                        int q = 0;
                        int w = 0;
                        q = Int32.Parse(Variables[n].value);
                        w = Int32.Parse(Variables[m].value);
                        switch (Operator)
                        {
                            case '>':
                                if (numone > w)
                                {
                                    LoopCon = true;
                                }
                                break;
                            case '<':
                                if (numone < w)
                                {
                                    LoopCon = true;
                                }
                                break;
                            case '%':
                                if (numone % w == 0)
                                {
                                    LoopCon = true;
                                }
                                break;
                        }
                    }
                }
            }
        }

        void ChangeVariable(string text, int LineNo,string WhichV)
        {
            int pos = 0;
            for (int i = 0; i < Variables.Count; i++)
            {
                if (Variables[i].name == WhichV)
                {
                    pos = i;
                    break;
                }
            }
            richTextBox3.AppendText("Line " + (LineNo+1) + " : ");
            richTextBox3.AppendText("\r\n");
            while (Tokens.Count > 0)
            {
                Token Temp = Tokens.Pop();
                switch (Temp.Type)
                {
                    case "V":
                        richTextBox3.AppendText(Temp.value + " Is " + "Variable");
                        richTextBox3.AppendText("\r\n");
                        break;
                    case "N":
                        Variables[pos].value = Temp.value;
                        break;
                    case "Num":
                        richTextBox3.AppendText(Temp.value + " Is " + "Number");
                        richTextBox3.AppendText("\r\n");
                        break;
                    case "Op":
                        richTextBox3.AppendText(Temp.value + " Is " + "Operator");
                        richTextBox3.AppendText("\r\n");
                        break;
                    case "CV":
                        richTextBox3.AppendText(Temp.value + " Is " + "Variable");
                        richTextBox3.AppendText("\r\n");
                        break;
                }

            }
            richTextBox3.AppendText("--------------------------------");
            richTextBox3.AppendText("\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetLineDesign();
            Colourit();
        }

        void Colourit()
        {
            
            int SelectBack = richTextBox1.SelectionStart;
            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = richTextBox1.Text.Length;
            richTextBox1.SelectionColor = Color.Black;
            for (int i = 0; i < ReservedWords.Length; i++)
            {
                
                int FBS = 0;
                while (true)
                {
                    string mystring = ReservedWords[i];
                    
                    
                    int my1stPosition = richTextBox1.Find(mystring, FBS,richTextBox1.Text.Length, new RichTextBoxFinds());
                    if (my1stPosition != -1 && my1stPosition<richTextBox1.Text.Length)
                    {
                        richTextBox1.SelectionStart = my1stPosition;

                        richTextBox1.SelectionLength = mystring.Length;

                        richTextBox1.SelectionColor = Color.Blue;

                        //  richTextBox1.SelectionStart = 0;
                        //  richTextBox1.SelectionLength = 0;
                        FBS=my1stPosition + mystring.Length;
                        
                    }
                    else
                        break;

                    if (FBS == richTextBox1.Text.Length)
                        break;
                }

            }
            richTextBox1.SelectionStart = SelectBack;
            richTextBox1.SelectionLength = 0;
            
            
        }

    }
}
