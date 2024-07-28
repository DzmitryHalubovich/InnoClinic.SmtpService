using InnoClinic.SharedModels.MQMessages.Appointments;
using InnoClinic.SharedModels.MQMessages.IdentityServer;
using InnoClinic.SharedModels.MQMessages.Profiles;

namespace NotificationService.API.Services;

public interface IEmailService
{
    public Task SendAppointmentApprovedNotification(AppointmentApprovedMessage message);

    public Task SendAppointmentRemindNotification(AppointmentRemindNotificationMessage message);

    public Task SendAppointmentResultCreated(AppointmentResultCreatedMessage message);

    public Task SendAppointmentResultUpdated(AppointmentResultUpdatedMessage message);

    public Task SendDoctorProfileCreated(WorkerProfileRegisteredMessage message);

    public Task SendEmailConfirmation(EmailConfirmationMessage message);
}
