// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: CalculateResponse.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Resonance.Messages.Proto {

  /// <summary>Holder for reflection information generated from CalculateResponse.proto</summary>
  public static partial class CalculateResponseReflection {

    #region Descriptor
    /// <summary>File descriptor for CalculateResponse.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CalculateResponseReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChdDYWxjdWxhdGVSZXNwb25zZS5wcm90bxIPVGFuZ28uUE1SLlN0dWJzIiAK",
            "EUNhbGN1bGF0ZVJlc3BvbnNlEgsKA1N1bRgBIAEoAUIbChljb20udHdpbmUu",
            "dGFuZ28ucG1yLnN0dWJzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Resonance.Messages.Proto.CalculateResponse), global::Resonance.Messages.Proto.CalculateResponse.Parser, new[]{ "Sum" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class CalculateResponse : pb::IMessage<CalculateResponse> {
    private static readonly pb::MessageParser<CalculateResponse> _parser = new pb::MessageParser<CalculateResponse>(() => new CalculateResponse());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CalculateResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Resonance.Messages.Proto.CalculateResponseReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CalculateResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CalculateResponse(CalculateResponse other) : this() {
      sum_ = other.sum_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CalculateResponse Clone() {
      return new CalculateResponse(this);
    }

    /// <summary>Field number for the "Sum" field.</summary>
    public const int SumFieldNumber = 1;
    private double sum_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double Sum {
      get { return sum_; }
      set {
        sum_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CalculateResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CalculateResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Sum != other.Sum) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Sum != 0D) hash ^= Sum.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Sum != 0D) {
        output.WriteRawTag(9);
        output.WriteDouble(Sum);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Sum != 0D) {
        size += 1 + 8;
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CalculateResponse other) {
      if (other == null) {
        return;
      }
      if (other.Sum != 0D) {
        Sum = other.Sum;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 9: {
            Sum = input.ReadDouble();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code