using Producer.Infrastructure.Config;

namespace Producer.Business.Services.Interface;

public interface IFaultInjectionService
{
    public FaultPhase GetRandomPhase();
    public bool ShouldDropRecord();
    public void MaybeCorruptTail(string path);
}