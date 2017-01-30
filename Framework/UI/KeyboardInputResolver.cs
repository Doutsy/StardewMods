using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Input;

using StardewValley;

namespace Entoarox.Framework.UI
{
	static class KeyboardInputResolver
	{
        /// <summary>
        /// This event is triggered when a given key is first pressed
        /// Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account
        /// </summary>
		static public event Action<Keys> KeyDown;
        /// <summary>
        /// This event is triggered for a key that is held down long enough to require a repeat firing
        /// Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account
        /// This will not fire every update, as a internal counter is used to keep repeat firing at a acceptable rate
        /// </summary>
		static public event Action<Keys> KeyHeld;
        /// <summary>
        /// This event is triggered when a given key is released
        /// Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account
        /// </summary>
		static public event Action<Keys> KeyUp;
        /// <summary>
        /// This event is trigger when a given key is first pressed or held
        /// It outputs the char for the intended value rather then the Keys enumeration for the actual key pressed
        /// </summary>
		static public event Action<char> KeyPressed;
        // Private fields
        static private KeyboardState @old;
        static private Dictionary<Keys, int[]> Counter = new Dictionary<Keys, int[]>();
        static private bool @Shift = false;
        static private bool @Caps = false;
        static private bool @Alt = false;
        // Initializer, as there is some init needed
        static KeyboardInputResolver()
        {
            // Shared logic - hook the update
            StardewModdingAPI.Events.GameEvents.UpdateTick += Update;
            // MonoGame logic
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // Using reflection to hook the event so that monogame is not required to compile
                // Also makes sure the reference sticks just in case mono rewrite goes heads up on it
                Game1.game1.Window.GetType().GetEvent("TextInput").AddEventHandler(Game1.game1.Window, (Action<object, EventArgs>)TextInputHandler);
                return;
            }
            // XNA logic
            KeyDown += KeyDownHandler;
            KeyUp += KeyUpHandler;
            KeyHeld += KeyHeldHandler;
        }
        // The method responsible for handling the update
        static private void Update(object s, EventArgs e)
		{
			KeyboardState @new=Keyboard.GetState();
			Keys[] @oldDown = @old.GetPressedKeys();
			Keys[] @down = @new.GetPressedKeys().Where(a => !@old.IsKeyDown(a)).ToArray();
			Keys[] @up = @oldDown.Where(a => !@new.IsKeyDown(a)).ToArray();
			Keys[] @held = @oldDown.Where(a => @new.IsKeyDown(a)).ToArray();
			foreach(Keys @key in @down)
			{
				KeyDown?.Invoke(@key);
				Counter.Add(@key,new int[2]{30,30});
			}
			foreach(Keys @key in @up)
			{
				Counter.Remove(@key);
				KeyUp?.Invoke(@key);
			}
			foreach(Keys @key in @held)
			{
				Counter[@key][0]--;
				if(Counter[@key][0]!=0)
					continue;
				Counter[@key][0]=Counter[@key][1];
				Counter[@key][1]=Math.Max(Counter[@key][1]-1,15);
				KeyHeld?.Invoke(@key);
			}
			@old=@new;
		}
		// if XNA is being used, we need to do quite a bit of logic to make sure that the correct characters are output
		static private void KeyDownHandler(Keys @key)
		{
			switch(@key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    @Shift =true;
					@Alt=true;
					break;
				case Keys.CapsLock:
					@Caps=true;
					@Alt=true;
					break;
			}
			KeyPressed?.Invoke(ResolveChar(@key));
		}
		static private void KeyUpHandler(Keys @key)
		{
			switch(@key)
			{
				case Keys.LeftShift:
                case Keys.RightShift:
                    @Shift =false;
					@Alt = @Caps;
					break;
				case Keys.CapsLock:
					@Caps=false;
					@Alt = @Shift;
					break;
			}
		}
		static private void KeyHeldHandler(Keys @key)
		{
			KeyPressed?.Invoke(ResolveChar(@key));
		}
		static private char ResolveChar(Keys @key)
		{
			char @char=(char)@key;
			if(!@Alt)
				return @char;
            short @pre = VkKeyScan(@char);
			uint @post=(uint)@pre&0xFF;
			byte[] @arr = new byte[256];
			@arr[0x10]=0x80;
			uint @out;
            ToAscii(@post, @post, @arr, out @out, 0);
			return (char)@out;
		}
		[DllImport("user32.dll")]
		static extern short VkKeyScan(char c);

		[DllImport("user32.dll", SetLastError=true)]
		static extern int ToAscii(
			uint uVirtKey,
			uint uScanCode,
			byte[] lpKeyState,
			out uint lpChar,
			uint flags
			);
		// If MonoGame is in use, we can easily take advantage of its build-in input handler
		static private void TextInputHandler(object s, EventArgs e)
		{
            KeyPressed?.Invoke((char)e.GetType().GetField("Character").GetValue(e));
		}
	}
}