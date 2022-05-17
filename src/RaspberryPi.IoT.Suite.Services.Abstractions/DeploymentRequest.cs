namespace RaspberryPi.IoT.Suite.Services.Abstractions;

public record DeploymentRequest<TDeploymentOption>(DeploymentType DeploymentType, TDeploymentOption DeploymentOption) where TDeploymentOption : class, IDeploymentOption;