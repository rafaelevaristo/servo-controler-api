using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;



[ApiController]
[Route("api/[controller]")]
public class ServoController : ControllerBase
{

    private static SerialPort _serialPort;
    private readonly SerialSettings _settings;

    public ServoController(IConfiguration configuration)
    {
        // Lê a secção "SerialConfig" do appsettings.json e mapeia para a classe
        _settings = configuration.GetSection("SerialConfig").Get<SerialSettings>() 
                    ?? new SerialSettings();
    }
    [HttpGet("portas")]
    public IActionResult ListarPortas()
    {
        try
        {
            // Obtém a lista de nomes das portas série ativas no sistema
            string[] portas = SerialPort.GetPortNames();
            
            return Ok(new { 
                total = portas.Length, 
                disponiveis = portas 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao listar portas: {ex.Message}");
        }
    }

    [HttpPost("mover")]
public IActionResult MoverServo([FromBody] ServoCommand command)
{
    try 
    {
        if (_serialPort == null) {
            _serialPort = new SerialPort(_settings.PortName, _settings.BaudRate);
            _serialPort.Open();
            System.Threading.Thread.Sleep(2000); // Só espera uma vez na vida
        }
        
        if (!_serialPort.IsOpen) _serialPort.Open();

        _serialPort.WriteLine($"{command.Velocidade},{command.Tempo}");
        return Ok("Comando enviado!");
    }
    catch (Exception ex) { return BadRequest(ex.Message); }
}
}