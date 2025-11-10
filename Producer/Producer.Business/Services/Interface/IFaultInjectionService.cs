using Producer.Common.Types;

namespace Producer.Business.Services.Interface;

public interface IFaultInjectionService
{
    public FaultPhaseType GetRandomPhase();
    public bool ShouldDropRecord();
    public void MaybeCorruptTail(string path);
}