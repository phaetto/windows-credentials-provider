namespace LogServer.Contexts.ConsoleLog.Commands.Data
{
    using Serviceable.Objects.Remote.Serialization;

    public sealed class LogData : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public string Text { get; set; }
    }
}
