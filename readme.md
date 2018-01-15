TextFileParsers: Easy structured text files parsing for .Net
=================================

Clone of [TextFileParsers (CodePlex)](https://textfileparsers.codeplex.com)

TextFileParsers is a library written C# that makes it easier for programmers to parse structured text files, both delimited and fixed width text files.

It is inspired in the TextFieldParser class from VisualBasic.FileIO namespace but refactored to allow better extensibility.

Remarks
--------

This library contains a small hierarchy of classes that allow to easily parse structured text files but also sets the bases for implementing a specialized field parser if needed.

The included classes are:
StructuredTextParser is an abstract base class from which both DelimitedFieldParser and FixedWidthFieldParser are derived. Specialized parsers just need to override the ReadFields method to provide their own implementation.
DelimitedFieldParser provides methods and properties for parsing delimited text files such as comma separated values files (csv) or tab delimited files often used for the interchange of information between applications.
FixedWidthFieldParser provides methods and properties for parsing structured text files, such as logs, whose fields have fixed widths.
TextFields provides access to the field values within each line.

Parsing a text file with both DelimitedFieldParser and FixedWidthFieldParser is similar to iterating over a text file, using the ReadFields method to read fields values.

Basic usage guides

Read from a delimited text file
--------

To parse a comma-delimited file that has some fields enclosed in quotes:

```C#
using (DelimitedFieldParser parser = new DelimitedFieldParser("contacts.csv"))
{
	parser.SetDelimiters(',');
	parser.HasFieldsEnclosedInQuotes = true;
	
	while (!parser.EndOfFile)
	{
		try
		{
			// Reads the next record.
			TextFields fields = parser.ReadFields();
			
			contact = new ContactInfo();

			// Gets parsed data.
			contact.Name = fields.GetString(0);
			contact.Email = fields.GetString(1);
			contact.Birthday = fields.GetDateTime(2);

			contactList.Add(contact);
		}
		catch (MalformedLineException e)
		{
			// Handle exception here.
		}
	}
}
```

Read from a fixed-width text file
--------

To parse a fixed-width text file:

```C#
using (FixedWidthFieldParser parser = new FixedWidthFieldParser("error.log"))
{
	parser.SetFieldWidths(3, 20, -1);
	//
	while (!parser.EndOfFile)
	{
		try
		{
			// Reads the next record
			TextFields fields = parser.ReadFields();
			
			// Gets parsed data
			error.Code = fields.GetInt32(0);
			error.Message = fields.GetString(1);
			error.source = fields.GetString2);
			...
		}
		catch (MalformedLineException e)
		{
			// Handle exception here.
		}
	}
}
```
