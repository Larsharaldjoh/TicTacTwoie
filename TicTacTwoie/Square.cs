using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TicTacTwoie
{
	enum State { Blank, X, O };
	class Square
	{
		private State state;
		public Rectangle rectangle;
		public int row;
		public int column;
		public Square(Rectangle rect, int r, int c)
		{
			rectangle = rect;
			row = r;
			column = c;
		}

		public void setState(State s)
		{
			state = s;
		}
		public State getState()
		{
			return state;
		}
		public bool stateEquals(State s)
		{
			return (s == state);
		}
	}
}
