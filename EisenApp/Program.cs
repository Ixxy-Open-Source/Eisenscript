// See https://aka.ms/new-console-template for more information

using Builder;


static class Foo
{
    public static void Main()
    {
        TextReader input = new StringReader(@"
r1
rule r1 {
  { x 1.3  } r1
  { s 1 } sphere
}
");
        var builder = new SSBuilder();
        builder.DrawEvent += (sender, args) =>
        {
            Console.WriteLine($"Sender: {sender} args: {args.Type} {args.Matrix.Translation}");
        };
        Console.WriteLine("ready...");
        builder.Build(input);
    }

}


