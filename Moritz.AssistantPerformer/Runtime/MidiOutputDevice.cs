
using System.Diagnostics;

using Multimedia.Midi;
using Moritz.Score.Midi;

namespace Moritz.AssistantPerformer.Runtime
{
    /// <summary>
    /// This node streams MIDI messages to an external device (e.g. a synthesizer) and can write
    /// MIDI files.
    /// Uses the internal classes MidiFileCreator.
    /// </summary>
    internal partial class MidiOutputDevice : IMidiSink
    {
        #region constructors
        public MidiOutputDevice(OutputDevice multimediaMidiOutputDevice)
        {
            _multimediaMidiOutputDevice = multimediaMidiOutputDevice;
        }
        #endregion constructors

        /// <summary>
        /// called when a chord arrives
        /// </summary>
        /// <param name="chord"></param>
        public void ProcessMessage(ChordMessage chord)
        {
            foreach(ChannelMessage message in chord.ChannelMessages)
            {
                ProcessMessage(message);
            }
        }
        public void ProcessMessage(ChannelMessage message)
        {
            if(_multimediaMidiOutputDevice != null)
               _multimediaMidiOutputDevice.Send(message);
            if(_fileCreator != null)
                _fileCreator.ProcessMessage(message);
        }
        public void ProcessMessage(SysExMessage message)
        {
            if(_multimediaMidiOutputDevice != null)
                _multimediaMidiOutputDevice.Send(message);
            if(_fileCreator != null)
                _fileCreator.ProcessMessage(message);
        }
        public void ProcessMessage(SysCommonMessage message)
        {
            if(_multimediaMidiOutputDevice != null)
                _multimediaMidiOutputDevice.Send(message);
            if(_fileCreator != null)
                _fileCreator.ProcessMessage(message);
        }
        public void ProcessMessage(SysRealtimeMessage message)
        {
            if(_multimediaMidiOutputDevice != null)
                _multimediaMidiOutputDevice.Send(message);
            if(_fileCreator != null)
                _fileCreator.ProcessMessage(message);
        }

        /// <summary>
        /// This MidiOutputDevice is connected to its IMidiSource.
        /// </summary>
        public void StartMidiStreaming(IMidiSource assistantPerformerRuntime, SaveSequenceAsMidiFileDelegate saveSequenceAsMidiFile, string defaultMidiFilename)
        {
            _assistantPerformerRuntime = assistantPerformerRuntime;
            _fileCreator = null;
            if(_multimediaMidiOutputDevice != null && _isRunning == false)
            {
                assistantPerformerRuntime.Connect(this);
                _isRunning = true;
                if(!string.IsNullOrEmpty(defaultMidiFilename))
                {
                    _fileCreator = new MidiFileCreator(this, saveSequenceAsMidiFile, defaultMidiFilename);
                    if(_fileCreator != null)
                        _fileCreator.StartRecording();
                }
                InitializeOutputDevice();
            }
        }
        private void InitializeOutputDevice()
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            builder.Command = ChannelCommand.Controller;

            for(int track = 0; track < 16; track++)
            {
                builder.MidiChannel = track;

                builder.Data1 = (int)ControllerType.AllSoundOff;
                builder.Build();
                _multimediaMidiOutputDevice.Send(builder.Result);

                builder.Data1 = (int)ControllerType.AllControllersOff;
                builder.Build();
                _multimediaMidiOutputDevice.Send(builder.Result);
            }
        }

        public void StopMidiStreaming()
        {   
            if(_isRunning)
            {
                if(_fileCreator != null)
                    _fileCreator.StopRecording();
                Debug.Assert(_assistantPerformerRuntime != null); 
                _assistantPerformerRuntime.Disconnect(this);
                _isRunning = false;
            }
        }

        private IMidiSource _assistantPerformerRuntime = null;
        private bool _isRunning = false;
        private MidiFileCreator _fileCreator; // created if _pathname is not empty
        private Multimedia.Midi.OutputDevice _multimediaMidiOutputDevice = null;

    }
}
