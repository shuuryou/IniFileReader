# C# INI File Reader Class

IniFileReader is a tiny .NET INI file parser written in C#. I wrote it for personal use, but I've also ended up using it in professionally released software that shipped thousands of copies.

Like most things, I created this out of frustration. Back when I needed a cross-platform INI parser, a web search for "C# ini parser" returned lots of projects with thousands of lines of code, interfaces, classes, dependencies on logging libraries, etc. All for parsing a *text file*. No wonder all software today is so bloated.

IniFileReader is a single, self-contained class with just a constructor and one method. It doesn't have any dependencies and should work with essentially any version of the .NET Framework. I've tested it with .NET Framework 2.0 and newer. The entire code base is just 138 lines with comments, or a little over 100 lines without comments. In fact, the filesize of this README file is larger than the file size of the class.

I place this work in the public domain. Do what you want with it.

## How to Use
It's easy:

1. Add the class to your project
1. Change the class namespace to your project's namespace
1. Create an instance of the class, passing the path to your INI file to the constructor
1. Retrieve settings using the `Get(section, key, valueIfMissing = null)` method

That's all you have to do.

## Notes

* The default encoding is UTF-8. If you need to use a different encoding like Shift-JIS or KOI8-R, just pass your desired encoding to the constructor.
* The INI file is cached in memory after it has been read. To reload it, just create a new instance of the class.
* The `Get` method will return null if the section or the key does not exist. Notably, it will never throw an exception as long as the `section` and `key` method parameters are not `null`.
* The `Get` method returns strings. Since INI files are usually designed to be edited by users, you are responsible for validating the data (e.g.  `int.TryParse(...)` for numbers, etc.).
* An `InvalidDataException` is thrown if the INI file contains garbage, like a setting outside of a section.
* Values spanning multiple lines are not supported, even if quoted.
* Section and key names are not case-sensitive.

## Usage Example

```ini
; This is my INI file. Hello.

; If you uncomment this line, it will cause an InvalidDataException
; InvalidSetting=I am outside of a section.

[Some Settings]
; Here's a comment about some settings.
Type=Box
Color=Red
Length=20
Width=40
Height=10
Purpose=

; Some whitespace doesn't bother the parser.

Description=This is a red box.
DescriptionJA="赤い箱です。" ; This is Japanese
; Next setting is missing a closing quote, so the inline comment becomes part of the value.
DescriptionNL="Dit is een rode kist. ; This is Dutch.

[OtherSettings]
Hello=Hello World
Duplicate=Will you get me?
Duplicate=Or will you get me?

; The following is considered valid. The section and key will be "" (empty strings).
[]
=Test

```

```cs
class Program
{
    static void Main(string[] args)
    {
        IniFileReader ini = new IniFileReader("settings.ini");

        Console.WriteLine(ini.Get("Some Settings", "Type"));
        Console.WriteLine(ini.Get("Some Settings", "Color"));
        Console.WriteLine(ini.Get("Some Settings", "Length"));
        Console.WriteLine(ini.Get("Some Settings", "Width"));
        Console.WriteLine(ini.Get("Some Settings", "Height"));
        Console.WriteLine(ini.Get("Some Settings", "Purpose"));

        Console.WriteLine(ini.Get("Some Settings", "Description"));
        Console.WriteLine(ini.Get("Some Settings", "DescriptionJA"));
        Console.WriteLine(ini.Get("Some Settings", "DescriptionNL"));

        Console.WriteLine(ini.Get("OtherSettings", "Hello"));
        Console.WriteLine(ini.Get("OtherSettings", "Duplicate"));
        Console.WriteLine(ini.Get("OtherSettings", "IDontExist", "I am the default if missing"));

        Console.WriteLine(ini.Get("ThisSectionDoesntExist", "Setting123") ?? "Returned NULL");

        /* Output:
         * Box
         * Red
         * 20
         * 40
         * 10
         * 
         * This is a red box. 
         * 赤い箱です。
         * "Dit is een rode kist. ; This is Dutch.
         * Hello World
         * Or will you get me?
         * I am the default if missing
         * Returned NULL
         */
    }
}
 ```
