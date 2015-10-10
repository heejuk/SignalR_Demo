using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR_Demo1.Controllers;

namespace SignalR_Demo1.Models
{
    public class Board
    {
        private List<Card> _pieces = new List<Card>();
        public List<Card> Pieces
        {
            get { return _pieces; }
            set { _pieces = value; }
        }

        public Board()
        {
            string[] easyLevel = {"가", "나", "다", "라", "마", "바", "사", "아", "자", "차", "카", "타", "파", "하", "네"};

            int imgIndex = 1;
            for (int i = 1; i <= 30; i++)
            {
                if (IsOdd(i))
                    _pieces.Add(new Card() 
                    { 
                        Id = i,
                        Pair = i+1,
                        Name = "card-" + i.ToString(),
                        Image = easyLevel[imgIndex-1]
                        //Image = string.Format("/content/img/{0}.png", imgIndex)
                    });
                else
                {
                    _pieces.Add(new Card()
                    {
                        Id = i,
                        Pair = i - 1,
                        Name = "card-" + i.ToString(),
                        Image = easyLevel[imgIndex-1]
                        //Image = string.Format("/content/img/{0}.png", imgIndex)
                    });
                    imgIndex++;
                }
            }
            _pieces.Shuffle();
        }

        private bool IsOdd(int i)
        {
            return i % 2 != 0;
        }
    }
}