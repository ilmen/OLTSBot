using System;

namespace OurLittleTastyServerBot.Classes.Telegram.Models
{
    public class JsonWebhookInfo
    {
        public string Url { get; set; }

        public bool has_custom_certificate { get; set; }
            
        public Int32 pending_update_count { get; set; }     // ���������� ����������, ��������� ��������
            
        public Int32? last_error_date { get; set; }         // �����������. Unix-����� ����� ��������� ������ �������� ���������� �� ��������� ������
            
        public string last_error_message { get; set; }      // �����������. �������� � ���������������� ������� ��������� ������ �������� ���������� �� ��������� ������
    }
}