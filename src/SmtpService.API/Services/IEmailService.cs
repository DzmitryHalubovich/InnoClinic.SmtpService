using InnoClinic.SharedModels.MQMessages.Appointments;

namespace SmtpAPI.Services;

public interface IEmailService
{
    public Task SendAppointmentApprovedNotification(AppointmentApprovedMessage message);

    public Task SendAppointmentRemindNotification(AppointmentRemindNotificationMessage message);

    public Task SendAppointmentResultCreated(AppointmentResultCreatedMessage message);

    public Task SendAppointmentResultUpdated(AppointmentResultUpdatedMessage message);
}
