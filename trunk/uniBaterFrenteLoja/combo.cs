using System;
using System.Collections.Generic;
using System.Text;

namespace uniBaterFrenteLoja
{
    class combo
    {
        public combo(string nome, string value)
        {
            Nome = nome;
            Value = value;
        }

        private string _nome;

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }
        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public combo()
        { 
        
        }
    }
    
}
